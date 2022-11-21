using AutoMapper;
using Barwy.Data.Data.Repositories.Interfaces;
using Barwy.Data.Data.ViewModels.User;

namespace Barwy.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var usersVM = new List<UserVM>();

            foreach (var user in users)
            {
                var userVM = _mapper.Map<UserVM>(user);
                var roles = await _userRepository.GetRolesAsync(user);
                userVM.Role = roles.First();
                usersVM.Add(userVM);
            }

            return new ServiceResponse
            {
                Message = "All users successfully loaded.",
                IsSuccess = true,
                Payload = usersVM
            };
        }
    }
}
