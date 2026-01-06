using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class PrinterService
    {
        public bool PrintTableOrder(TableOrder order, string? printerName = null, string currencySymbol = "$", Dictionary<string, string>? localizedStrings = null, bool onlyNewItems = true)
        {
            try
            {
                var itemsToPrint = onlyNewItems
                    ? order.OrderItems.Where(i => !i.IsPrinted).ToList()
                    : order.OrderItems.ToList();

                if (itemsToPrint.Count == 0)
                {
                    System.Windows.MessageBox.Show("No new items to print.");
                    return false;
                }

                string receipt = GenerateReceiptText(order, itemsToPrint, currencySymbol, localizedStrings);

                if (System.OperatingSystem.IsWindows())
                {
                    try
                    {
                        System.Windows.Controls.PrintDialog printDialog = new();
                        if (printDialog.ShowDialog() == true)
                        {
                            System.Windows.MessageBox.Show("Printing to: " + (printerName ?? "System Default") + "\n\n" + receipt, "Printer Output");
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(receipt, $"Order Receipt - Table {order.TableNumber} (Print Error: {ex.Message})");
                        return true;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(receipt, $"Order Receipt - Table {order.TableNumber}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Print error: {ex.Message}");
                return false;
            }
        }

        public void PrintReceiptDirect(TableOrder order, string printerName, string currencySymbol = "$", Dictionary<string, string>? localizedStrings = null)
        {
            try
            {
                string receiptText = GenerateReceiptText(order, order.OrderItems.ToList(), currencySymbol, localizedStrings);
                System.Windows.MessageBox.Show(receiptText, $"Order - Table {order.TableNumber}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Print error: {ex.Message}");
            }
        }

        private string GenerateReceiptText(TableOrder order, List<OrderItem> itemsToPrint, string currencySymbol = "$", Dictionary<string, string>? localizedStrings = null)
        {
            const int width = 32;
            string header = localizedStrings?.GetValueOrDefault("Header") ?? "RESTAURANT ORDER";
            string tableLabel = localizedStrings?.GetValueOrDefault("Table") ?? "Table";
            string timeLabel = localizedStrings?.GetValueOrDefault("Time") ?? "Time";
            string totalLabel = localizedStrings?.GetValueOrDefault("Total") ?? "TOTAL";
            string thankYou = localizedStrings?.GetValueOrDefault("ThankYou") ?? "THANK YOU!";

            var lines = new List<string>
            {
                "================================",
                CenterText(header, width),
                "================================",
                "",
                $"{tableLabel}: {order.TableNumber} ({order.TableLocation})",
                $"{timeLabel}:  {order.OrderedAt:dd/MM/yyyy HH:mm:ss}",
                "",
                "--------------------------------"
            };

            foreach (OrderItem item in itemsToPrint)
            {
                string itemLine = $"{item.Quantity}x {item.MenuItem.Name}";
                if (itemLine.Length > width - 10)
                    itemLine = itemLine.Substring(0, width - 13) + "...";

                string pricePart = currencySymbol == "$" ? $"${item.MenuItem.Price:F2}" : $"{item.MenuItem.Price:F2} {currencySymbol}";
                lines.Add($"{itemLine,-20} {pricePart,10}");

                if (item.SelectedToppings.Count > 0)
                {
                    foreach (Topping topping in item.SelectedToppings)
                    {
                        string toppingLine = $"  + {topping.Name}";
                        if (toppingLine.Length > width - 10)
                            toppingLine = toppingLine.Substring(0, width - 13) + "...";

                        string toppingPricePart = currencySymbol == "$" ? $"${topping.Price:F2}" : $"{topping.Price:F2} {currencySymbol}";
                        lines.Add($"{toppingLine,-20} {toppingPricePart,10}");
                    }
                }
            }

            lines.Add("--------------------------------");
            decimal total = itemsToPrint.Sum(i => i.GetTotal());
            string totalAmountStr = currencySymbol == "$" ? $"${total:F2}" : $"{total:F2} {currencySymbol}";
            string totalLine = $"{totalLabel}: {totalAmountStr}";
            lines.Add(totalLine.PadLeft(width));
            lines.Add("================================");
            lines.Add("");

            return string.Join("\n", lines);
        }

        private string CenterText(string text, int width)
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            return text.PadLeft(text.Length + leftPadding).PadRight(width);
        }

        public List<string> GetAvailablePrinters()
        {
            if (System.OperatingSystem.IsWindows())
            {
                return GetWindowsPrinters();
            }
            return new List<string>();
        }

        private List<string> GetWindowsPrinters()
        {
            List<string> printers = new();
            try
            {
                System.Printing.LocalPrintServer printServer = new();
                foreach (System.Printing.PrintQueue queue in printServer.GetPrintQueues())
                {
                    printers.Add(queue.Name);
                }
            }
            catch
            {
                //ignore if printing subsystem is not available
            }
            return printers;
        }
    }
}
