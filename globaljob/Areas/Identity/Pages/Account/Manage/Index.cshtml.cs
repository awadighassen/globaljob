using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using globaljob.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace globaljob.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public string[] GovernatesList { get; set; } = new string[24] { "Ariana", "Béja", "Ben Arous", "Bizerte", "Gabès", "Gafsa", "Jendouba", "Kairouan", "Kasserine", "Kébili", "Le Kef", "Mahdia", "La Manouba", "Médenine", "Monastir", "Nabeul", "Sfax", "Sidi Bouzid", "Siliana", "Sousse", "Tataouine", "Tozeur", "Tunis", "Zaghouan" };
        public string Username { get; set; }

        public bool Role { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "First name field is required")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            public string DateBirth { get; set; }

            [Required(ErrorMessage = "Adress field is required")]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Governate field is required")]
            [Display(Name = "Governate")]
            public string Governate { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            if (CheckRole(user))
            {
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateBirth = user.DateBirth,
                    Address = user.Address,
                    Governate = user.Governate
                };
            }
            else
            {
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    FirstName = user.FirstName,
                    Address = user.Address,
                    Governate = user.Governate
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            //var user = _userManager.FindByIdAsync(_userManager.GetUserId(HttpContext.User)).Result;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (CheckRole(user))
            {
                if (Input.FirstName != user.FirstName)
                {
                    user.FirstName = Input.FirstName;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.LastName != user.LastName)
                {
                    user.LastName = Input.LastName;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.DateBirth != user.DateBirth)
                {
                    user.DateBirth = Input.DateBirth;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Address != user.Address)
                {
                    user.Address = Input.Address;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Governate != user.Governate)
                {
                    user.Governate = Input.Governate;
                    await _userManager.UpdateAsync(user);
                }
            }
            else
            {
                if (Input.FirstName != user.FirstName)
                {
                    user.FirstName = Input.FirstName;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Address != user.Address)
                {
                    user.Address = Input.Address;
                    await _userManager.UpdateAsync(user);
                }

                if (Input.Governate != user.Governate)
                {
                    user.Governate = Input.Governate;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public bool CheckRole(ApplicationUser user)
        {
            Role = _userManager.IsInRoleAsync(user, "Chercheur").Result;
            return Role;

        }
    }
}
