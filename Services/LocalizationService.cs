using System.Collections.Generic;

namespace RestaurantApp.Services
{
    public class LocalizationService
    {
        private static LocalizationService? _instance;
        public static LocalizationService Instance => _instance ??= new LocalizationService();

        public enum Language
        {
            English,
            Turkish
        }

        private Language _currentLanguage = Language.English;

        public Language CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value;
        }

        public string FormatCurrency(decimal amount)
        {
            return _currentLanguage == Language.Turkish 
                ? $"{amount:F2} TL" 
                : $"${amount:F2}";
        }

        public string GetString(string key)
        {
            return _currentLanguage switch
            {
                Language.Turkish => GetTurkish(key),
                Language.English => GetEnglish(key),
                _ => GetEnglish(key)
            };
        }

        public string GetCurrencySymbol()
        {
            return _currentLanguage switch
            {
                Language.Turkish => "TL",
                Language.English => "$",
                _ => "$"
            };
        }

        private string GetEnglish(string key)
        {
            return key switch
            {
                // Main Window
                "Title" => "Restaurant Management System",
                "Header" => "🏪 Restaurant Management System",
                "SettingsButton" => "Settings",
                "EarningsButton" => "Earnings",
                "TablesLabel" => "Tables",
                "MainFoodsTab" => "Main Foods",
                "DrinksTab" => "Drinks",
                "ToppingsLabel" => "Toppings",
                "SelectedItemLabel" => "Selected Item",
                "QuantityLabel" => "Quantity",
                "AddToOrderButton" => "Add to Order",
                "CurrentOrderLabel" => "Current Order",
                "TotalLabel" => "Total",
                "PrintButton" => "Print",
                "PrintReceiptButton" => "Print Receipt",
                "CheckoutButton" => "Checkout",
                "CheckoutCreditCardButton" => "Credit Card",
                "CheckoutCashButton" => "Cash",
                "CheckoutPackageOrderButton" => "Package Order",
                "DeleteButton" => "Delete",
                "CancelButton" => "Cancel Order",
                "ConfirmCancel" => "Are you sure you want to cancel the entire order for this table?",
                "NoItemSelected" => "No item selected",
                "MonthLabel" => "Month",
                "Table" => "Table",
                "Time" => "Time",
                "Status" => "Status",
                "ItemsLabel" => "Items",
                "CloseButton" => "Close",

                // Settings Window
                "SettingsTitle" => "Settings",
                "GeneralTab" => "General",
                "MenuManagementTab" => "Menu Management",
                "TablesTab" => "Tables",
                "LanguageLabel" => "Language",
                "PrinterConfigLabel" => "Printer Configuration",
                "NetworkPrinterIPLabel" => "Network Printer IP Address",
                "NetworkPrinterPortLabel" => "Network Printer Port",
                "MenuItemsLabel" => "Menu Items",
                "EditItemLabel" => "Edit Item",
                "NameLabel" => "Name",
                "CategoryLabel" => "Category",
                "PriceLabel" => "Price",
                "AddToppingButton" => "Add Topping",
                "EditTableLabel" => "Edit Table",
                "DisplayNameLabel" => "Display Name",
                "TableNumberLabel" => "Table Number",
                "LocationLabel" => "Location",
                "SaveAllSettingsButton" => "Save All Settings",

                // Receipt
                "ReceiptHeader" => "RESTAURANT ORDER",
                "ReceiptTable" => "Table",
                "ReceiptTime" => "Time",
                "ReceiptTotal" => "TOTAL",
                "ReceiptThankYou" => "THANK YOU!",

                // Dialogs
                "ConfirmCheckout" => "Confirm checkout for this table?",
                "CheckoutSuccess" => "Order checked out",
                "PaymentMethodLabel" => "Payment Method",
                "PaymentMethodCreditCard" => "Credit Card",
                "PaymentMethodCash" => "Cash",
                "SelectTableFirst" => "Please select a table first",
                "SelectItemFirst" => "Please select an item first",
                "InvalidQuantity" => "Quantity must be at least 1",
                "PrintSuccess" => "Order printed successfully",
                "PrintError" => "Error printing order",

                // Table Display
                "TableNumber" => "Table",
                "PaymentMethod" => "Payment Method",
                "ActiveOrder" => "Active Order",
                "True" => "Yes",
                "False" => "No",
                "Occupied" => "Occupied",
                "Empty" => "Empty",
                "CheckedOut" => "Checked Out",
                "Cancelled" => "Cancelled",
                "Inside" => "Inside",
                "Outside" => "Outside",

                _ => key
            };
        }

        private string GetTurkish(string key)
        {
            return key switch
            {
                // Main Window
                "Title" => "RESTORAN YONETIM SISTEMI",
                "Header" => "🏪 RESTORAN YONETIM SISTEMI",
                "SettingsButton" => "AYARLAR",
                "EarningsButton" => "KAZANCLAR",
                "TablesLabel" => "MASALAR",
                "MainFoodsTab" => "ANA YEMEKLER",
                "DrinksTab" => "ICECEKLER",
                "ToppingsLabel" => "SOS VE EKSTRALAR",
                "SelectedItemLabel" => "SECILI URUN",
                "QuantityLabel" => "ADET",
                "AddToOrderButton" => "SIPARISE EKLE",
                "CurrentOrderLabel" => "GECERLI SIPARIS",
                "TotalLabel" => "TOPLAM",
                "PrintButton" => "SERVIS YAZDIR",
                "PrintReceiptButton" => "FIS YAZDIR",
                "CheckoutButton" => "ODEME AL",
                "CheckoutCreditCardButton" => "KART",
                "CheckoutCashButton" => "NAKIT",
                "CheckoutPackageOrderButton" => "PAKET",
                "DeleteButton" => "SIL",
                "CancelButton" => "SIPARISI IPTAL ET",
                "ConfirmCancel" => "BU MASA ICIN TUM SIPARISI IPTAL ETMEK ISTEDIGINIZDEN EMIN MISINIZ?",
                "NoItemSelected" => "URUN SECILMEDI",
                "MonthLabel" => "AY",
                "Table" => "MASA",
                "Time" => "SAAT",
                "Status" => "DURUM",
                "ItemsLabel" => "URUNLER",
                "CloseButton" => "KAPAT",

                // Settings Window
                "SettingsTitle" => "AYARLAR",
                "GeneralTab" => "GENEL",
                "MenuManagementTab" => "MENU YONETIMI",
                "TablesTab" => "MASALAR",
                "LanguageLabel" => "DIL",
                "PrinterConfigLabel" => "YAZICI YAPILANDIRMASI",
                "NetworkPrinterIPLabel" => "AG YAZICISI IP ADRESI",
                "NetworkPrinterPortLabel" => "AG YAZICISI PORTU",
                "MenuItemsLabel" => "MENU OGELERI",
                "EditItemLabel" => "OGEYI DUZENLE",
                "NameLabel" => "ISIM",
                "CategoryLabel" => "KATEGORI",
                "PriceLabel" => "FIYAT",
                "AddToppingButton" => "MALZEME EKLE",
                "EditTableLabel" => "MASAYI DUZENLE",
                "DisplayNameLabel" => "GORUNEN ISIM",
                "TableNumberLabel" => "MASA NUMARASI",
                "LocationLabel" => "KONUM",
                "SaveAllSettingsButton" => "TUM AYARLARI KAYDET",

                // Receipt
                "ReceiptHeader" => "RESTORAN SIPARISI",
                "ReceiptTable" => "MASA",
                "ReceiptTime" => "SAAT",
                "ReceiptTotal" => "TOPLAM",
                "ReceiptThankYou" => "TESEKKURLER!",

                // Dialogs
                "ConfirmCheckout" => "BU MASA ICIN ODEMELYI ONAYLA?",
                "CheckoutSuccess" => "SIPARIS TAMAMLANDI",
                "PaymentMethodLabel" => "ODEME YONTEMI",
                "PaymentMethodCreditCard" => "KREDI KARTI",
                "PaymentMethodCash" => "NAKIT",
                "SelectTableFirst" => "LUTFEN ONCE MASA SECIN",
                "SelectItemFirst" => "LUTFEN ONCE URUN SECIN",
                "InvalidQuantity" => "ADET EN AZ 1 OLMALIDIR",
                "PrintSuccess" => "SIPARIS BASARILY YAZDIRILDI",
                "PrintError" => "SIPARIS YAZDIRILIRKEN HATA OLUStu",

                // Table Display
                "TableNumber" => "MASA",
                "PaymentMethod" => "ODEME YONTEMI",
                "ActiveOrder" => "AKTIF SIPARIS",
                "True" => "EVET",
                "False" => "HAYIR",
                "Occupied" => "DOLU",
                "Empty" => "BOS",
                "CheckedOut" => "TAMAMLANDI",
                "Cancelled" => "IPTAL EDILDI",
                "Inside" => "IC MEKAN",
                "Outside" => "DIS MEKAN",

                _ => key
            };
        }
    }
}
