using LaundryCleaning.Models.Subscriptions;
using LaundryCleaning.Service.Common.Constants;

namespace LaundryCleaning.Service.GraphQL.Users.Subscriptions
{
    [ExtendObjectType(ExtendObjectTypeConstants.Subscription)]
    public class UserSubscriptions
    {
        [Subscribe]
        [Topic]
        public UserCreated OnUserCreated([EventMessage] UserCreated user)
        {
            return user;
        }

    }
}
