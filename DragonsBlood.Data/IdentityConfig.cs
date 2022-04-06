using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using DragonsBlood.Models.Roles;
using DragonsBlood.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace DragonsBlood.Data
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try {
                // Plug in your email service here to send an email.
                SmtpClient c = new SmtpClient("smtp.123-reg.co.uk");

                var mail = new MailMessage("noreply@dragonsbloodalliance.co.uk", message.Destination, message.Subject, message.Body);
                c.Credentials = new NetworkCredential("noreply@dragonsbloodalliance.co.uk", "39L6eOezkD!");
                c.Send(mail);
                return Task.FromResult(c);
            }
            catch(Exception e)
            {
                return Task.FromResult(e);
            }
        }

        public void Send(IdentityMessage message)
        {
            try
            {
                // Plug in your email service here to send an email.
                SmtpClient c = new SmtpClient("smtp.123-reg.co.uk");

                var mail = new MailMessage("noreply@dragonsbloodalliance.co.uk", message.Destination, message.Subject, message.Body);
                c.Credentials = new NetworkCredential("noreply@dragonsbloodalliance.co.uk", "39L6eOezkD!");
                c.Send(mail);
            }
            catch (Exception)
            {
                
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<IdentityResult> ChangeDisplayNameAsync(string userId, string displayName)
        {
            try {
                var user = await Store.FindByIdAsync(userId);

                ApplicationDbContext context = new ApplicationDbContext();
                var dbUser = context.Users.First(u => u.Id == userId);
                var chatUser = context.ChatUsers.First(c => c.UserName == dbUser.DisplayName);
                var nameExists = context.Users.Any(u => u.DisplayName == displayName);

                if (nameExists)
                    return IdentityResult.Failed("Display name is already in use");

                dbUser.DisplayName = displayName;
                chatUser.UserName = displayName;
                context.SaveChanges();
                return IdentityResult.Success;
            }
            catch(Exception e)
            {
                return new IdentityResult(e.Message);
            }
        }

        public IdentityResult ChangeDisplayName(string userId, string displayName)
        {
            try
            {
                var user = Store.FindByIdAsync(userId);

                ApplicationDbContext context = new ApplicationDbContext();
                var dbUser = context.Users.First(u => u.Id == userId);
                var chatUser = context.ChatUsers.First(c => c.UserName == dbUser.DisplayName);
                var nameExists = context.Users.Any(u => u.DisplayName == displayName);

                if (nameExists)
                    return IdentityResult.Failed("Display name is already in use");

                dbUser.DisplayName = displayName;
                chatUser.UserName = displayName;
                context.SaveChanges();

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return new IdentityResult(e.Message);
            }
        }

        public IdentityResult UpdateSettings(string userId, UserSettings settings)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var dbUser = context.Users.First(u => u.Id == userId);

                dbUser.Settings = settings;
                context.SaveChanges();
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return new IdentityResult(e.Message);
            }
        }        
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore) : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
