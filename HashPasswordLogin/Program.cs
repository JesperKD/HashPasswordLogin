using System;
using System.Diagnostics;
using System.IO;

namespace HashPasswordLogin
{
    class Program
    {
        static string input = "";
        static int iterations = 50000;
        static int loginAttempts = 5;
        static void Main()
        {
            bool running = true;
            while (running == true)
            {
                Console.Clear();
                Console.WriteLine("Choose a function: \n1. Login to Account \n2. Create Account \n3. Exit");
                int choice = Convert.ToInt32(Console.ReadLine());

                if (loginAttempts > 0)
                {

                    switch (choice)
                    {
                        case 1:
                            Login();
                            break;

                        case 2:
                            CreateAccount();
                            break;

                        case 3:
                            running = false;
                            break;
                    }
                    Console.WriteLine("Press any key...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("You've run out of login attempts...");
                    Console.ReadKey();
                }
            }
        }

        public static void CreateAccount()
        {
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Type in username:");
            string username = Console.ReadLine();

            Console.WriteLine("Type in your password:");
            input = Console.ReadLine();
            sw.Start();
            byte[] salt = Hash.GenerateSalt();
            var hashedPassword1 = Hash.ComputeHash(input, salt, iterations, salt.Length);
            sw.Stop();
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\crypto.txt", username + "," + Convert.ToBase64String(hashedPassword1) + "," + Convert.ToBase64String(salt) + "\n");
            Console.WriteLine();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms Passed to hash password.");
        }

        public static void Login()
        {
            string[] accountData = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\crypto.txt");

            string[] splitString = new string[32];

            Console.WriteLine("Insert Username:");
            string insertedUsrName = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine("Insert Password:");
            string insertedPass = Console.ReadLine();
            Console.WriteLine();

            foreach (string item in accountData)
            {
                if (item.Contains(insertedUsrName))
                {
                    splitString = item.Split(',');
                }
            }

            byte[] savedPass = Convert.FromBase64String(splitString[1]);
            byte[] salt = Convert.FromBase64String(splitString[2]);
            byte[] accountPass = Hash.ComputeHash(insertedPass, salt, iterations, salt.Length);


            if (ByteArrayCompare(savedPass, accountPass) == true)
            {

                Console.WriteLine("Login Succesfull.");
            }
            else
            {
                loginAttempts--;
                Console.WriteLine("Login failed. " + loginAttempts + " login attempts left.");
            }
        }

        static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
    }
}
