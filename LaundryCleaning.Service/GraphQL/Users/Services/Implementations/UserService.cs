using HotChocolate.Subscriptions;
using LaundryCleaning.Service.Common.Exceptions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Common.Models.Messages;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.GraphQL.Users.CustomModels;
using LaundryCleaning.Service.GraphQL.Users.Inputs;
using LaundryCleaning.Service.GraphQL.Users.Services.Interfaces;
using LaundryCleaning.Service.GraphQL.Users.Subscriptions;
using LaundryCleaning.Service.Services.Implementations;
using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace LaundryCleaning.Service.GraphQL.Users.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordService _passwordService;
        private readonly ITopicEventSender _topicEventSender;
        private readonly ILogger<UserService> _logger;
        private readonly IPublisherService _publisherService;

        public UserService(
            ApplicationDbContext dbContext,
            IPasswordService passwordService,
            ITopicEventSender topicEventSender,
            ILogger<UserService> logger,
            IPublisherService publisherService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
            _topicEventSender = topicEventSender;
            _logger = logger;
            _publisherService = publisherService;
        }

        public async Task<List<User>> GetUsers(CancellationToken cancellationToken)
        {
            var data = await (from u in _dbContext.Users
                              select u).ToListAsync(cancellationToken);

            return data;
        }

        public async Task<CreateUserCustomModel> CreateUser(CreateUserInput input, CancellationToken cancellationToken)
        {
            var emailExist = await _dbContext.Users.Where(x => x.Email.Equals(input.Email)).FirstOrDefaultAsync(cancellationToken);

            if (emailExist != null)
            {
                throw new BusinessLogicException("Email already used, please use another Email!.");
            }

            var newUser = new User() {
                Email = input.Email,
                Password = _passwordService.GeneratePassword(input.Password),
                Username = input.Username?? string.Empty,
                FirstName = input.FirstName ?? string.Empty,
                LastName = input.LastName ?? string.Empty
            };

            await _dbContext.Users.AddAsync(newUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("New User has been created");

            // send subscription
            await _topicEventSender.SendAsync(nameof(UserSubscriptions.OnUserCreated), new UserCreated($"New User {newUser.Email} has been created" ), cancellationToken);

            var response = new CreateUserCustomModel() { 
                Success = true,
                Data = new UserCreatedResponse() {
                    Email = newUser.Email,
                    Username = newUser.Username,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                }
            };
            return response;
        }

        public async Task<string> SendUserNotification(string input, CancellationToken cancellationtoken) 
        {
            _logger.LogInformation("Start SendUserNotification");

            var userNotification = new UserNotification
            {
                UserId = Guid.Parse("C0B3553A-5EE3-43C6-AD65-FF9FA34B3F66"),
                Title = "New Message",
                Message = input
            };

            // Publish
            _logger.LogInformation("Start Publish SendUserNotification");
            await _publisherService.PublishAsync(userNotification, cancellationtoken);

            return "Send Notif Published";
        }
    }
}
