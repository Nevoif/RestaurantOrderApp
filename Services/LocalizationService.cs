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
                ? $"{amount:F2} ₺" 
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
                Language.Turkish => "₺",
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
                "CheckoutButton" => "Checkout",
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
                "SelectTableFirst" => "Please select a table first",
                "SelectItemFirst" => "Please select an item first",
                "InvalidQuantity" => "Quantity must be at least 1",
                "PrintSuccess" => "Order printed successfully",
                "PrintError" => "Error printing order",

                // Table Display
                "TableNumber" => "Table",
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
                "Title" => "Restoran Yönetim Sistemi",
                "Header" => "🏪 Restoran Yönetim Sistemi",
                "SettingsButton" => "Ayarlar",
                "EarningsButton" => "Kazançlar",
                "TablesLabel" => "Masalar",
                "MainFoodsTab" => "Ana Yemekler",
                "DrinksTab" => "İçecekler",
                "ToppingsLabel" => "Sos ve Ekstralar",
                "SelectedItemLabel" => "Seçili Ürün",
                "QuantityLabel" => "Adet",
                "AddToOrderButton" => "Siparişe Ekle",
                "CurrentOrderLabel" => "Geçerli Sipariş",
                "TotalLabel" => "Toplam",
                "PrintButton" => "Yazdır",
                "CheckoutButton" => "Ödeme Al",
                "DeleteButton" => "Sil",
                "CancelButton" => "Siparişi İptal Et",
                "ConfirmCancel" => "Bu masa için tüm siparişi iptal etmek istediğinizden emin misiniz?",
                "NoItemSelected" => "Ürün seçilmedi",
                "MonthLabel" => "Ay",
                "Table" => "Masa",
                "Time" => "Saat",
                "Status" => "Durum",
                "ItemsLabel" => "Ürünler",
                "CloseButton" => "Kapat",

                // Settings Window
                "SettingsTitle" => "Ayarlar",
                "GeneralTab" => "Genel",
                "MenuManagementTab" => "Menü Yönetimi",
                "TablesTab" => "Masalar",
                "LanguageLabel" => "Dil",
                "PrinterConfigLabel" => "Yazıcı Yapılandırması",
                "MenuItemsLabel" => "Menü Öğeleri",
                "EditItemLabel" => "Öğeyi Düzenle",
                "NameLabel" => "İsim",
                "CategoryLabel" => "Kategori",
                "PriceLabel" => "Fiyat",
                "AddToppingButton" => "Malzeme Ekle",
                "EditTableLabel" => "Masayı Düzenle",
                "DisplayNameLabel" => "Görünen İsim",
                "TableNumberLabel" => "Masa Numarası",
                "LocationLabel" => "Konum",
                "SaveAllSettingsButton" => "Tüm Ayarları Kaydet",

                // Receipt
                "ReceiptHeader" => "RESTORAN SİPARİŞİ",
                "ReceiptTable" => "Masa",
                "ReceiptTime" => "Saat",
                "ReceiptTotal" => "TOPLAM",
                "ReceiptThankYou" => "TEŞEKKÜRLER!",

                // Dialogs
                "ConfirmCheckout" => "Bu masa için ödemeyi onayla?",
                "CheckoutSuccess" => "Sipariş tamamlandı",
                "SelectTableFirst" => "Lütfen önce masa seçin",
                "SelectItemFirst" => "Lütfen önce ürün seçin",
                "InvalidQuantity" => "Adet en az 1 olmalıdır",
                "PrintSuccess" => "Sipariş başarıyla yazdırıldı",
                "PrintError" => "Sipariş yazdırılırken hata oluştu",

                // Table Display
                "TableNumber" => "Masa",
                "ActiveOrder" => "Aktif Sipariş",
                "True" => "Evet",
                "False" => "Hayır",
                "Occupied" => "Dolu",
                "Empty" => "Boş",
                "CheckedOut" => "Tamamlandı",
                "Cancelled" => "İptal Edildi",
                "Inside" => "İç Mekan",
                "Outside" => "Dış Mekan",

                _ => key
            };
        }
    }
}
