// See https://aka.ms/new-console-template for more information
using System;

class Program
{
    static void Main()
    {
        JalankanDenganDelegates();
        JalankanDenganEventArg();
    }

    public static void JalankanDenganDelegates()
    {
        Console.WriteLine("Ini jalan dengan delegates");
        var supermarket = new Supermarket();
        var pelanggan1 = new Pelanggan("Ghinna");
        var pelanggan2 = new Pelanggan("Naida");

        supermarket.Promo += pelanggan1.TerimaPromo;
        supermarket.Promo += pelanggan2.TerimaPromo;

        supermarket.UmumkanPromo("1 Kg Mangga", 20000);

        supermarket.Promo -= pelanggan2.TerimaPromo;
        supermarket.UmumkanPromo("1 Kg Apel", 25000);

        supermarket.Promo -= pelanggan1.TerimaPromo;
        supermarket.UmumkanPromo("1 Kg Jeruk", 15000);
    }

    public static void JalankanDenganEventArg()
    {
        Console.WriteLine("Ini jalan dengan Eventargs");
        var supermarket2 = new SupermarketEvent();
        var pelanggan3 = new Pelanggan("Putri");
        var pelanggan4 = new Pelanggan("Aprianto");

        supermarket2.Promo2 += pelanggan3.TerimaPromo;
        supermarket2.Promo2 += pelanggan4.TerimaPromo;

        supermarket2.UmumkanPromo("1 Kg Mangga", 20000);

        supermarket2.Promo2 -= pelanggan3.TerimaPromo;
        supermarket2.UmumkanPromo("1 Kg Apel", 25000);

        supermarket2.Promo2 -= pelanggan4.TerimaPromo;
        supermarket2.UmumkanPromo("1 Kg Jeruk", 15000);
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

        public void BersihkanEvent()
        {
            Promo = null; //ini ga kepake karna diprogram ini object short-lived dan ga static, tapi kalo dicase static, atau aplikasi berjalan terus menerus better kasih = null
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
        public void TerimaPromo(object? supermarket, PromoEventArgs e)
        {
            Console.WriteLine($"👤 {namaPelanggan} menerima promo: {e.Promo2} dengan harga Rp {e.HargaPromo}");
        }
    }

    public class PromoEventArgs : System.EventArgs
    {
        public readonly string Promo2;
        public readonly int HargaPromo;

        public PromoEventArgs(string promo2, int hargaPromo)
        {
            Promo2 = promo2;
            HargaPromo = hargaPromo;
        }
    }

    public class SupermarketEvent
    {
        public event EventHandler<PromoEventArgs>? Promo2;

        protected virtual void OnPromo(string promo, int harga)
        {
            Promo2?.Invoke(this, new PromoEventArgs(promo, harga));
        }

        public void UmumkanPromo(string promo, int harga)
        {
            Console.WriteLine($"📢 Supermarket sedang promo: {promo} menjadi harga Rp {harga}! Dapatkan segera !!");
            OnPromo(promo, harga);
        }

        public void BersihkanEvent()
        {
            Promo2 = null; //ini ga kepake karna diprogram ini object short-lived dan ga static, tapi kalo dicase static, atau aplikasi berjalan terus menerus better kasih = null
        }
    }

     
}