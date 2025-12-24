using System;
using System.Linq;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public enum PaymentPopupResult
{
    Pay,
    CancelOrder,
    Closed
}

public partial class PaymentPopupPage : ContentPage
{
    private readonly TaskCompletionSource<PaymentPopupResult> _tcs = new();
    private readonly Order _order;

    private bool _isFormatting;

    public Task<PaymentPopupResult> ResultTask => _tcs.Task;

    public string CardNumber => CardNumberEntry.Text?.Trim() ?? "";
    public string Expiry => ExpiryEntry.Text?.Trim() ?? "";
    public string Cvc => CvcEntry.Text?.Trim() ?? "";
    public string Holder => HolderEntry.Text?.Trim() ?? "";

    public PaymentPopupPage(Order order)
    {
        InitializeComponent();
        _order = order;

        OrderInfoLabel.Text = $"Заказ №{order.Id}, сумма: ₽{order.TotalAmount:F2}";
    }


    private void OnCardNumberTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isFormatting) return;

        _isFormatting = true;
        try
        {
            var rawDigits = DigitsOnly(e.NewTextValue);
            if (rawDigits.Length > 16)
                rawDigits = rawDigits.Substring(0, 16);

            var formatted = FormatCardNumber(rawDigits);

            var oldCursor = CardNumberEntry.CursorPosition;
            CardNumberEntry.Text = formatted;
            CardNumberEntry.CursorPosition = Math.Min(formatted.Length, oldCursor);
        }
        finally
        {
            _isFormatting = false;
        }
    }

    private void OnExpiryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isFormatting) return;

        _isFormatting = true;
        try
        {
            var rawDigits = DigitsOnly(e.NewTextValue);
            if (rawDigits.Length > 4)
                rawDigits = rawDigits.Substring(0, 4);

            var formatted = rawDigits.Length switch
            {
                <= 2 => rawDigits,
                _ => rawDigits.Substring(0, 2) + "/" + rawDigits.Substring(2)
            };

            var oldCursor = ExpiryEntry.CursorPosition;

            ExpiryEntry.Text = formatted;

            if (formatted.Length == 3 && oldCursor == 2)
                ExpiryEntry.CursorPosition = 3;
            else
                ExpiryEntry.CursorPosition = Math.Min(formatted.Length, oldCursor);
        }
        finally
        {
            _isFormatting = false;
        }
    }

    private void OnCvcTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isFormatting) return;

        _isFormatting = true;
        try
        {
            var rawDigits = DigitsOnly(e.NewTextValue);
            if (rawDigits.Length > 3)
                rawDigits = rawDigits.Substring(0, 3);

            CvcEntry.Text = rawDigits;
            CvcEntry.CursorPosition = Math.Min(rawDigits.Length, CvcEntry.CursorPosition);
        }
        finally
        {
            _isFormatting = false;
        }
    }

    private void OnCardNumberCompleted(object sender, EventArgs e) => ExpiryEntry.Focus();
    private void OnExpiryCompleted(object sender, EventArgs e) => CvcEntry.Focus();
    private void OnCvcCompleted(object sender, EventArgs e) => HolderEntry.Focus();
    private void OnHolderCompleted(object sender, EventArgs e) => OnPayClicked(sender, e);


    private async void OnPayClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (!IsValidCardData(out var error))
        {
            ErrorLabel.Text = error;
            ErrorLabel.IsVisible = true;
            return;
        }

        _tcs.TrySetResult(PaymentPopupResult.Pay);
        await Navigation.PopModalAsync();
    }

    private async void OnCancelOrderClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(PaymentPopupResult.CancelOrder);
        await Navigation.PopModalAsync();
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        _tcs.TrySetResult(PaymentPopupResult.Closed);
        await Navigation.PopModalAsync();
    }

    private bool IsValidCardData(out string error)
    {
        error = "";

        var digits = DigitsOnly(CardNumber);
        if (digits.Length != 16)
        {
            error = "Номер карты должен содержать 16 цифр.";
            return false;
        }

        if (Cvc.Length != 3 || !Cvc.All(char.IsDigit))
        {
            error = "CVC должен содержать 3 цифры.";
            return false;
        }

        // Expiry в формате MM/YY
        if (Expiry.Length != 5 || Expiry[2] != '/')
        {
            error = "Введите срок действия в формате MM/YY.";
            return false;
        }

        var mmStr = Expiry.Substring(0, 2);
        var yyStr = Expiry.Substring(3, 2);

        if (!int.TryParse(mmStr, out var mm) || !int.TryParse(yyStr, out var yy))
        {
            error = "Введите срок действия в формате MM/YY.";
            return false;
        }

        if (mm < 1 || mm > 12)
        {
            error = "Месяц должен быть от 01 до 12.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Holder))
        {
            error = "Введите имя держателя.";
            return false;
        }

        return true;
    }

    private static string DigitsOnly(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return new string(input.Where(char.IsDigit).ToArray());
    }

    private static string FormatCardNumber(string digits)
    {
        if (string.IsNullOrEmpty(digits))
            return string.Empty;

        var groups = Enumerable.Range(0, (digits.Length + 3) / 4)
            .Select(i => digits.Substring(i * 4, Math.Min(4, digits.Length - i * 4)));

        return string.Join(" ", groups);
    }
}
