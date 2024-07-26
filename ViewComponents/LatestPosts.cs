using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class LatestPosts : ViewComponent
    {
        private readonly IPostRepository _postRepository;

        public LatestPosts(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            List<Post> myPosts= await _postRepository
                .Posts
                .OrderByDescending(p => p.PublishedOn)
                .Take(5)
                .ToListAsync();


            return View(myPosts);
        }

    }
}
