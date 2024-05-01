using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Common;
using Common.Exception;
using Common.Utilities;
using Data;
using Data.Repositories;
using Data.Repositories.UserRepositories;
using Entities.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace WebFramework.Configuration
{
    public static class ServiceCollectionExtensions
    {

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                }).ConfigureWarnings(warning => warning.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
            });
        }

        //public static void AddMinimalMvc(this IServiceCollection services)
        //{
        //    services.AddMvcCore(options =>
        //        {
        //            options.Filters.Add(new AuthorizeFilter());
        //        })
        //        .AddApiExplorer()
        //        .AddAuthorization()
        //        .AddFormatterMappings()
        //        .AddDataAnnotations()
        //        .AddJsonFormatters(options =>
        //        {
        //            options.Formatting = Newtonsoft.Json.Formatting.Indented;
        //            options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //        }).AddCors()
        //        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        //}

        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context => throw new AppException(ApiResultStatusCode.UnAuthenticated, "عدم احراز هویت", HttpStatusCode.Unauthorized, context.Exception),
                    OnChallenge = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthenticated, "خطای احراز هویت", HttpStatusCode.Unauthorized, context.AuthenticateFailure);
                        throw new AppException(ApiResultStatusCode.UnAuthenticated, "شما برای دسترسی به منابع احراز هویت نشده اید", HttpStatusCode.Unauthorized);

                        //return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                        var userTokenRepository = context.HttpContext.RequestServices.GetRequiredService<IBaseRepository<UserToken>>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity != null && claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var clientId = claimsIdentity.GetClientId();
                        var userToken = await userTokenRepository.GetAll(x => x.ClientId == clientId).Include(x => x.User)
                            .Select(x => new UserToken { User = new User { IsActive = x.User.IsActive } })
                            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

                        if (userToken == null)
                            throw new AppException(ApiResultStatusCode.UnAuthenticated, "عدم احراز هویت", HttpStatusCode.Unauthorized);

                        if (!userToken.User.IsActive)
                            context.Fail("کاربری شما غیرفعال شده است");

                        //await userRepository.UpdateLastLoginDateAsync(userToken.User, context.HttpContext.RequestAborted);
                    }
                };

            });
        }


        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

        }
    }
}
