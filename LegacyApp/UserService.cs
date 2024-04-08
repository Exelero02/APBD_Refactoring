using System;

namespace LegacyApp
{
    public class UserService
    {
        private readonly ClientRepository _clientRepository;
        private readonly UserCreditService _userCreditService;

        public UserService()
        {
            _clientRepository = new ClientRepository();
            _userCreditService = new UserCreditService();
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsvalidInput(firstName, lastName, email, dateOfBirth))
                return false;

            var client = _clientRepository.GetById(clientId);

            var creditLimit = CalculateCreditLimit(client, lastName, dateOfBirth);

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client, creditLimit);

            SaveUser(user);

            return true;
        }

        private static bool IsvalidInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                return false;

            if (!email.Contains("@") || !email.Contains("."))
                return false;

            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now < dateOfBirth.AddYears(age))
                age--;

            return age >= 21;
        }

        private int CalculateCreditLimit(Client client, string lastName, DateTime dateOfBirth)
        {
            if (client.Type == "VeryImportantClient")
                return 0;
            else if (client.Type == "ImportantClient")
                return _userCreditService.GetCreditLimit(lastName, dateOfBirth) * 2;
            else
                return _userCreditService.GetCreditLimit(lastName, dateOfBirth);
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client, int creditLimit)
        {
            return new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName,
                HasCreditLimit = creditLimit > 0,
                CreditLimit = creditLimit
            };
        }

        private static void SaveUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}
