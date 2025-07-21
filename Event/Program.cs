// See https://aka.ms/new-console-template for more information
using System;

class Program
{
    static void Main()
    {
        var supermarket = new Supermarket();
        var pelanggan1 = new Pelanggan("Ghinna");
        var pelanggan2 = new Pelanggan("Naida");

        supermarket.Promo += pelanggan1.TerimaPromo;
        supermarket.Promo += pelanggan2.TerimaPromo;

        supermarket.UmumkanPromo("1 Kg Mangga", 20000);

        // supermarket.Promo -= pelanggan2.TerimaPromo;
        // supermarket.UmumkanPromo("1 Kg Apel", 25000);
    }

    public delegate void PromoEventHandler(object? sender, string promo, int harga);

    public class Supermarket
    {
        public event PromoEventHandler? Promo;

        public void UmumkanPromo(string promo, int harga)
        {
            Console.WriteLine($"📢 Supermarket sedang promo: {promo} menjadi harga Rp {harga}! Dapatkan segera !!");
            Promo?.Invoke(this, promo, harga);
        }
    }

    public class Pelanggan
    {
        public string namaPelanggan { get; }
        public Pelanggan(string nama)
        {
            namaPelanggan=nama;
        }

        public void TerimaPromo(object? supermarket, string promo, int harga)
        {
            Console.WriteLine($"👤 {namaPelanggan} menerima promo: {promo}");
        }
    }
}