// See https://aka.ms/new-console-template for more information
using System;

delegate int OperasiMatematika(int a, int b);

class Program
{
    
    
    static void Main()
    {
        CobaDelegates();
    }

    #region Delegates

    static int Tambah (int a, int b) => a + b;
    static int Kurang (int a, int b) => a - b;
    static int Kali (int a, int b) => a * b;
    static int Bagi (int a, int b) => a / b;

    static void JalankanOperasi(OperasiMatematika operasi, int a, int b)
    {
        Console.WriteLine($"{operasi.Method.Name} {a} & {b}\nHasil = {operasi(a, b)}");
    }

    static void JalankanOperasiDuaKali(OperasiMatematika operasi, int a, int b)
    {
        operasi += operasi;

        var hasil = operasi.GetInvocationList();
        int hasil1 = (int)hasil[0].DynamicInvoke(a,b);
        int hasil2 = (int)hasil[1].DynamicInvoke(a,b);
        Console.WriteLine($"{hasil[0].Method.Name} {a} & {b}\nHasil pertama = {hasil1}\n{hasil[1].Method.Name} {a} & {b}\nHasil kedua = {hasil2}");
    }
    public class Operasi{
        public int Tambahkan (int a, int b) => a + b;
        public int Kurangkan (int a, int b) => a - b;
        public static int Kalikan (int a, int b) => a * b;
        public static int Bagikan (int a, int b) => a / b;
    }
    static void CobaDelegates()
    {
        JalankanOperasi(Tambah, 10, 5);
        JalankanOperasiDuaKali(Tambah, 10, 5);
        JalankanOperasi(Kurang, 10, 5);
        JalankanOperasi(Kali, 10, 5);
        JalankanOperasi(Bagi, 10, 5);

        Operasi operasi = new Operasi();
        JalankanOperasi(operasi.Tambahkan, 10, 5);
        JalankanOperasi(operasi.Kurangkan, 10, 5);
        JalankanOperasi(Operasi.Kalikan, 10, 5);
        JalankanOperasi(Operasi.Bagikan, 10, 5);
    }
    #endregion


    #region Event Handler
    #endregion
}