using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string  UserName { get; set; }

        [Required]
        [Display(Name = "Adınız")]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [Required]
        [StringLength(10,ErrorMessage = "{0} alanı en az {2} karakterden oluşmalıdır.",MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]

        public string? Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parola Tekrar")]
        [Compare(nameof(Password),ErrorMessage = "Parolanız eşleşmiyor.")]
        public string? ConfirmPassword { get; set; }

    }
}
