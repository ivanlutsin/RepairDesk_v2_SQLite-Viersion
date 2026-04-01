using ReactiveUI;
using System.Reactive;
using RepairDesk.Models;
using RepairDesk.Services;
using YourApp.Services;
using System;

namespace RepairDesk.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _sellerCode = "";
        public string SellerCode
        {
            get => _sellerCode;
            set => this.RaiseAndSetIfChanged(ref _sellerCode, value);
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public event EventHandler<User>? LoginSuccessful;

        public MainWindowViewModel()
        {
            var userService = new UserService();

            LoginCommand = ReactiveCommand.Create(() =>
            {
                IsLoading = true;
                ErrorMessage = "";

                try
                {
                    Console.WriteLine($"=== LOGIN ATTEMPT ===");
                    Console.WriteLine($"Entered code: '{SellerCode}' (length: {SellerCode?.Length})");
                    
                    var user = userService.GetBySellerCode(SellerCode?.Trim());

                    if (user == null)
                    {
                        ErrorMessage = "Invalid employee code";
                        Console.WriteLine($"=== ACCESS DENIED ===");
                    }
                    else
                    {
                        Console.WriteLine($"=== ACCESS GRANTED: {user.FullName} ({user.Role}) ===");
                        LoginSuccessful?.Invoke(this, user);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: {ex.Message}";
                    Console.WriteLine($"[ERROR] {ex}");
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }
    }
}