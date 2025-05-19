using Microsoft.EntityFrameworkCore;
using SBM.API.Data;
using SBM.API.Services;
using SBM.Shared.Entities;
using SBM.Shared.Responses;

namespace Fantasy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IApiService _apiService;

    public SeedDb(DataContext context, IApiService apiService)
    {
        _context = context;
        _apiService = apiService;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        //await CheckTeamsAsync();
        //await CheckRolesAsync();
        //await CheckUserAsync("Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", UserType.Admin);
    }

    //private async Task CheckRolesAsync()
    //{
    //    await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
    //    await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    //}

    //private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, UserType userType)
    //{
    //    var user = await _usersUnitOfWork.GetUserAsync(email);
    //    if (user == null)
    //    {
    //        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Colombia");
    //        user = new User
    //        {
    //            FirstName = firstName,
    //            LastName = lastName,
    //            Email = email,
    //            UserName = email,
    //            PhoneNumber = phone,
    //            Country = country!,
    //            UserType = userType,
    //        };

    //        await _usersUnitOfWork.AddUserAsync(user, "123456");
    //        await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

    //        var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
    //        await _usersUnitOfWork.ConfirmEmailAsync(user, token);
    //    }

    //    return user;
    //}

    //private async Task CheckCountriesAsync()
    //{
    //    if (!_context.Countries.Any())
    //    {
    //        var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
    //        await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
    //    }
    //}
    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            GenericResponse responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
            if (responseCountries.IsSuccess)
            {
                List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;
                foreach (CountryResponse countryResponse in countries)
                {
                    Country? country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;
                    if (country == null)
                    {
                        country = new() { Name = countryResponse.Name!, Iso2 = countryResponse.Iso2!, PhoneCode = countryResponse.PhoneCode!, States = new List<State>() };
                        GenericResponse responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                        if (responseStates.IsSuccess)
                        {
                            List<StateResponse> states = (List<StateResponse>)responseStates.Result!;
                            foreach (StateResponse stateResponse in states!)
                            {
                                State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;
                                if (state == null)
                                {
                                    state = new() { Name = stateResponse.Name!, Cities = new List<City>() };
                                    GenericResponse responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                    if (responseCities.IsSuccess)
                                    {
                                        List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;
                                        foreach (CityResponse cityResponse in cities)
                                        {
                                            if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                            {
                                                continue;
                                            }
                                            City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;
                                            if (city == null)
                                            {
                                                state.Cities.Add(new City() { Name = cityResponse.Name! });
                                            }
                                        }
                                    }
                                    if (state.CitiesNumber > 0)
                                    {
                                        country.States.Add(state);
                                    }
                                }
                            }
                        }
                        if (country.StatesNumber > 0)
                        {
                            _context.Countries.Add(country);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
    //private async Task CheckTeamsAsync()
    //{
    //    if (!_context.Teams.Any())
    //    {
    //        foreach (var country in _context.Countries)
    //        {
    //            var imagePath = string.Empty;
    //            var filePath = $"{Environment.CurrentDirectory}\\Images\\Flags\\{country.Name}.png";
    //            if (File.Exists(filePath))
    //            {
    //                var fileBytes = File.ReadAllBytes(filePath);
    //                imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "teams");
    //            }
    //            _context.Teams.Add(new Team { Name = country.Name, Country = country!, Image = imagePath });
    //        }

    //        await _context.SaveChangesAsync();
    //    }
    //}
}