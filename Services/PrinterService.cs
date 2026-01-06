using RestaurantApp.Models;
using System.Net.Sockets;
using System.Text;

namespace RestaurantApp.Services
{
    public class PrinterService
    {
        // Network printer settings
        private string _networkPrinterIP = "192.168.1.100";  // Change to your printer's IP
        private int _networkPrinterPort = 9100;  // Standard ESC/POS port

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

                // Try network printing first (for Ethernet printers)
                if (!string.IsNullOrEmpty(printerName) && printerName.Contains("(Network)"))
                {
                    bool networkPrintResult = PrintToNetworkPrinter(receipt);
                    if (networkPrintResult)
                        return true;
                }

                // Fall back to Windows printing
                if (System.OperatingSystem.IsWindows())
                {
                    try
                    {
                        System.Windows.Controls.PrintDialog printDialog = new();
                        if (!string.IsNullOrEmpty(printerName))
                        {
                            printDialog.PrintQueue = new System.Printing.LocalPrintServer()
                                .GetPrintQueues()
                                .FirstOrDefault(q => q.Name == printerName);
                        }

                        if (printDialog.ShowDialog() == true)
                        {
                            System.Windows.MessageBox.Show("Printing to: " + (printerName ?? "System Default"), "Print Success");
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

                // Try network printing if it's a network printer
                if (printerName.Contains("(Network)"))
                {
                    PrintToNetworkPrinter(receiptText);
                }
                else
                {
                    System.Windows.MessageBox.Show(receiptText, $"Order - Table {order.TableNumber}");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Print error: {ex.Message}");
            }
        }

        private bool PrintToNetworkPrinter(string receiptText)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.ConnectAsync(_networkPrinterIP, _networkPrinterPort).Wait(5000);

                    if (!client.Connected)
                    {
                        System.Windows.MessageBox.Show($"Cannot connect to printer at {_networkPrinterIP}:{_networkPrinterPort}. Make sure the IP is correct and the printer is powered on.");
                        return false;
                    }

                    using (NetworkStream stream = client.GetStream())
                    {
                        List<byte> commands = new();

                        // ESC/POS initialization
                        commands.AddRange(new byte[] { 0x1B, 0x40 });  // ESC @ (Initialize)

                        // Set alignment to left (default)
                        commands.AddRange(new byte[] { 0x1B, 0x61, 0x00 });  // ESC a (Left alignment)

                        // Set character size - 3x height, 2x width for bigger text
                        commands.AddRange(new byte[] { 0x1B, 0x21, 0x21 });  // ESC ! (height=2, width=1, magnified)

                        // Convert text to bytes and send
                        byte[] data = Encoding.UTF8.GetBytes(receiptText);
                        commands.AddRange(data);

                        // Reset character size to normal
                        commands.AddRange(new byte[] { 0x1B, 0x21, 0x00 });  // ESC ! (Normal size)

                        // Add blank lines before cut
                        commands.AddRange(Encoding.UTF8.GetBytes("\n\n\n"));

                        // ESC/POS full cut paper
                        commands.AddRange(new byte[] { 0x1B, 0x6D });  // ESC m (Full cut)

                        // Send all commands at once
                        byte[] allCommands = commands.ToArray();
                        stream.Write(allCommands, 0, allCommands.Length);
                        stream.Flush();

                        // Small delay to ensure paper is cut before closing connection
                        System.Threading.Thread.Sleep(500);
                    }

                    System.Windows.MessageBox.Show("Receipt sent to printer successfully!");
                    return true;
                }
            }
            catch (TimeoutException)
            {
                System.Windows.MessageBox.Show($"Timeout: Cannot reach printer at {_networkPrinterIP}:{_networkPrinterPort}");
                return false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Network print error: {ex.Message}");
                return false;
            }
        }

        public void SetNetworkPrinterIP(string ip, int port = 9100)
        {
            _networkPrinterIP = ip;
            _networkPrinterPort = port;
        }

        private string GenerateReceiptText(TableOrder order, List<OrderItem> itemsToPrint, string currencySymbol = "$", Dictionary<string, string>? localizedStrings = null)
        {
            const int width = 42;  // Wider receipt (80mm thermal printer standard)
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
                $"{tableLabel}: {order.TableNumber,-6} | {order.TableLocation}",
                $"{timeLabel}: {order.OrderedAt:dd/MM/yyyy HH:mm:ss}",
                "",
                "--------------------------------"
            };

            foreach (OrderItem item in itemsToPrint)
            {
                string itemLine = $"{item.Quantity}x {item.MenuItem.Name}";
                if (itemLine.Length > width - 12)
                    itemLine = itemLine.Substring(0, width - 15) + "...";

                string pricePart = currencySymbol == "$" ? $"${item.MenuItem.Price}" : $"{item.MenuItem.Price}";
                lines.Add($"{itemLine,-28}"); //deleted pricePart,12


                if (item.SelectedToppings.Count > 0)
                {
                    foreach (Topping topping in item.SelectedToppings)
                    {
                        string toppingLine = $"  + {topping.Name}";
                        if (toppingLine.Length > width - 12)
                            toppingLine = toppingLine.Substring(0, width - 15) + "...";

                        string toppingPricePart = currencySymbol == "$" ? $"${topping.Price}" : $"{topping.Price} {currencySymbol}"; //deleted :F2 after topping.Price
                        lines.Add($"{toppingLine,-28} "); // DeletedtoppingPricePart,12
                    }
                }
            }

            lines.Add("-------------------------------");
            decimal total = itemsToPrint.Sum(i => i.GetTotal());
            string totalAmountStr = currencySymbol == "$" ? $"${total}" : $"{total} {currencySymbol}"; //deleted :F2 after total 2 times.
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
            List<string> printers = new();

            // Add network printer option
            printers.Add("Ethernet Printer (Network)");

            // Add Windows printers
            if (System.OperatingSystem.IsWindows())
            {
                printers.AddRange(GetWindowsPrinters());
            }

            return printers;
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
