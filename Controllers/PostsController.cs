using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;


namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITagRepository _tagRepository;


        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult>Index(string tag)
        {


            var posts = _postRepository.Posts.Where(i => i.IsActive); 

            if (!string.IsNullOrEmpty(tag))
            {
                    posts = posts
                    .Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return View(await posts.Include(x => x.Tags).ToListAsync());



        }

        public async Task<IActionResult> PostDetail(string url)
        {
           Post post= await _postRepository
               .Posts
               .Include(x => x.User)
               .Include(x=>x.Tags)
               .Include(x=>x.Comments)
               .ThenInclude(x=>x.User)
               .FirstOrDefaultAsync(p => p.Url == url);

           return View(post);
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(int PostId,string Text, string Url)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userImage = User.FindFirstValue(ClaimTypes.UserData);

            var entity = new Comment()
            {
                CommentText = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = int.Parse(userId??"")
            };

            _commentRepository.CreateComment(entity);

            return Json(new
            {
                userName,
                Text,
                entity.PublishedOn,
                userImage
            });
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public IActionResult Create(PostCreateViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {

                _postRepository.CreatePost(new Post
                {
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url,
                    Image = "1.jpg",
                    UserId = int.Parse(userId),
                    PublishedOn = DateTime.Now,
                    IsActive = false
                }) ;

                return RedirectToAction("Index");

            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;

            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(p => p.UserId == userId);
            }


            return View(await posts.ToListAsync());
        }


        [Authorize]
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = _postRepository.Posts.Include(i=>i.Tags).FirstOrDefault(i => i.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Tags = _tagRepository.Tags.ToList();
            
            return View(new PostCreateViewModel()
            {
                PostId = post.PostId,
                Content = post.Content,
                Description = post.Description,
                IsActive = post.IsActive,
                Title = post.Title,
                Url = post.Url,
                Tags=post.Tags,
            });


        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(PostCreateViewModel model, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var entityToUpdate = new Post()
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url
                };

                if (User.FindFirstValue(ClaimTypes.Role)=="admin")
                {
                    entityToUpdate.IsActive = model.IsActive;
                }

                _postRepository.EditPost(entityToUpdate, tagIds);
            }
            return RedirectToAction("List");

        }



    }
}
