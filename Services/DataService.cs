using System.IO;
using System.Text.Json;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class DataService
    {
        private readonly string _dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RestaurantApp"
        );

        private readonly string _settingsFile;
        private readonly string _ordersFile;
        private readonly string _checkoutsDirectory;

        public DataService()
        {
            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);

            _settingsFile = Path.Combine(_dataDirectory, "settings.json");
            _ordersFile = Path.Combine(_dataDirectory, "orders.json");
            _checkoutsDirectory = Path.Combine(_dataDirectory, "checkouts");

            if (!Directory.Exists(_checkoutsDirectory))
                Directory.CreateDirectory(_checkoutsDirectory);
        }

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    string json = File.ReadAllText(_settingsFile);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? GetDefaultSettings();
                }
            }
            catch { }

            return GetDefaultSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFile, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}");
            }
        }

        public List<TableOrder> LoadOrders()
        {
            try
            {
                if (File.Exists(_ordersFile))
                {
                    string json = File.ReadAllText(_ordersFile);
                    return JsonSerializer.Deserialize<List<TableOrder>>(json) ?? new();
                }
            }
            catch { }

            return new();
        }

        public void SaveOrders(List<TableOrder> orders)
        {
            try
            {
                string json = JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_ordersFile, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving orders: {ex.Message}");
            }
        }

        public void SaveCheckout(TableOrder order)
        {
            try
            {
                var checkoutTime = order.CheckedOutAt ?? DateTime.Now;
                order.CheckedOutAt = checkoutTime;

                string monthFolder = Path.Combine(_checkoutsDirectory, checkoutTime.ToString("yyyy-MM"));
                if (!Directory.Exists(monthFolder))
                    Directory.CreateDirectory(monthFolder);


                //save
                string fileName = $"Checkout_{order.TableNumber}_{checkoutTime:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(monthFolder, fileName);
                string json = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                //update
                string monthlyFile = Path.Combine(_checkoutsDirectory, $"{checkoutTime:yyyy-MM}.json");
                List<TableOrder> monthlyCheckouts = new();
                if (File.Exists(monthlyFile))
                {
                    string monthlyJson = File.ReadAllText(monthlyFile);
                    monthlyCheckouts = JsonSerializer.Deserialize<List<TableOrder>>(monthlyJson) ?? new();
                }
                monthlyCheckouts.Add(order);
                string updatedMonthlyJson = JsonSerializer.Serialize(monthlyCheckouts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(monthlyFile, updatedMonthlyJson);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving checkout: {ex.Message}");
            }
        }

        public List<TableOrder> LoadMonthlyCheckouts(int year, int month)
        {
            try
            {
                string monthlyFile = Path.Combine(_checkoutsDirectory, $"{year}-{month:D2}.json");
                if (File.Exists(monthlyFile))
                {
                    string json = File.ReadAllText(monthlyFile);
                    return JsonSerializer.Deserialize<List<TableOrder>>(json) ?? new();
                }
            }
            catch { }
            return new();
        }

        public List<string> GetAvailableMonths()
        {
            try
            {
                if (!Directory.Exists(_checkoutsDirectory))
                    return new();

                return Directory.GetFiles(_checkoutsDirectory, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(n => n != null && n.Length == 7 && n.Contains("-"))
                    .OrderByDescending(n => n)
                    .ToList()!;
            }
            catch { return new(); }
        }

        private AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                TotalTables = 4,
                InsideTables = 2,
                OutsideTables = 2,
                MenuItems = new()
            };
        }
    }
}
