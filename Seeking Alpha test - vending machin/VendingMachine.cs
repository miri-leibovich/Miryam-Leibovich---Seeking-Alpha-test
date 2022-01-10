using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Seeking_Alpha_test___vending_machin
{
    public class VendingMachine
    {
        public List<Drink> Drinks { get; set; }
        public List<Coin> Coins { get; set; }
        private VendingMachine machine;

        //The method is responsible for the operation of the machine
        public void On()
        {
            string jsonPath = "../../../vendingMachine.json";
            string selectedDrink;

            if (LoadMachine(jsonPath))
            {

                //If method LoadMachine() succeeded it would happen.
                while (true)
                {
                    PrintMenu();

                    //Input from the customer who chooses a drink.
                    selectedDrink = Console.ReadLine();

                    //Receive a print message from the method GetMassegeForSelectedDrink() .
                    string selectedDrinkMessage = GetMassegeForSelectedDrink(selectedDrink);

                    //Print the received message.
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(selectedDrinkMessage);
                    Console.ResetColor();

                    //Stop iterating in case the input was not good.
                    if (selectedDrinkMessage == "You have a typo, try again..." || selectedDrinkMessage == "Out of stock")
                        continue;

                    //If the input is valid now it is possible to convert it.
                    int selectedDrinkIndex = int.Parse(selectedDrink);
                    Console.WriteLine("You can pay with the following coins");
                    PrintCoins(machine.Coins.Select(c => c.Value).ToList());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("If you want to stop buying, press any character and you will get your money back");
                    Console.ResetColor();

                    //Check if the payment was successful (the customer paid all the money).
                    if (!Payment(machine.Drinks[selectedDrinkIndex - 1].Price))
                        continue;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Your order");
                    Thread.Sleep(1000);
                    Console.WriteLine("---");
                    Thread.Sleep(1000);
                    Console.WriteLine(machine.Drinks[selectedDrinkIndex - 1].ProductName);
                    Console.ResetColor();
                    machine.Drinks[selectedDrinkIndex - 1].Quantity--;
                    if (!UpdateMachineStock())
                        break;
                }
            }

            //If the machine fails it will happen.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("We have temporary problem, Please try again later");
            Console.ResetColor();
        }

        //A method that loads the data from the JSON file into the machine.
        public bool LoadMachine(string jsonPath)
        {
            try
            {
                using (StreamReader r = new StreamReader(jsonPath))
                {
                    string jsonString = r.ReadToEnd();
                    machine = JsonConvert.DeserializeObject<VendingMachine>(jsonString);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //The method shows all the types of drinks available in the machine.
        public void PrintMenu()
        {
            Thread.Sleep(1000);

            //Screen cleaning after each order.
            Console.Clear();
            Console.WriteLine("Hi! what do you want to drink?\nPress:");
            for (int i = 0; i < machine.Drinks.Count; i++)
            {
                Console.WriteLine(i + 1 + " for " + machine.Drinks[i].ProductName);
            }

        }

        //The method returns the most appropriate message depending on the input received.
        public string GetMassegeForSelectedDrink(string selectedDrink)
        {
            int selectedDrinkIndex;
            try
            {
                //Attempt to convert the input to int (If the input is a number - it will work).
                selectedDrinkIndex = int.Parse(selectedDrink);
            }
            catch (Exception)
            {
                //If the input is not an integer number it will reuturn this message.
                return "You have a typo, try again...";
            }

            //Check if the number entered is in the correct range.
            if (selectedDrinkIndex < 1 || selectedDrinkIndex > machine.Drinks.Count)
            {
                return "You have a typo, try again...";
            }

            //Check if the drink is in stock.
            if (machine.Drinks[selectedDrinkIndex - 1].Quantity > 0)
                return "You need to pay: " + machine.Drinks[selectedDrinkIndex - 1].Price;

            //If the drink is out of stock.
            return "Out of stock";

        }

        //The method prints all the coins from the list it received.
        public void PrintCoins(List<float> coins)
        {
            if (coins.Count == 0)
            {
                Console.WriteLine("You didn't entered any coin");
            }
            for (int i = 0; i < coins.Count; i++)
            {
                Console.WriteLine(coins[i]);
            }
        }

        //The method manages the payment.
        public bool Payment(float selectedDrinkPrice)
        {
            List<float> userCoins = new List<float>();
            float sum = 0;
            float currentCoin;
            List<float> l;

            //While loop as long as the user did not put in all the money.
            while (sum < selectedDrinkPrice)
            {

                //Attempt to convert input to float (The type of coins).
                try
                {
                    currentCoin = float.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    //If the input is invalid.
                    Console.WriteLine("You didn't pay all the money...");
                    Console.WriteLine("The machine returns your money");
                    Thread.Sleep(1000);
                    PrintCoins(userCoins);
                    return false;
                }
                var e = (from coin in machine.Coins
                         where coin.Value == currentCoin
                         select coin).FirstOrDefault() != null;
                if (!e)
                {
                    Console.WriteLine("The machine does not support such coin, try again...");
                    continue;
                }
                sum += currentCoin;
                userCoins.Add(currentCoin);
            }

            //After the customer has entered all the coins, add the coins to the machine.
            for (int i = 0; i < userCoins.Count; i++)
            {
                machine.Coins[machine.Coins.FindIndex(c => c.Value == userCoins[i])].Quantity++;
            }

            //Check whether the customer should receive a surplus.
            if (sum > selectedDrinkPrice)
            {
                l = GetSurplus(sum - selectedDrinkPrice);
                Console.WriteLine("Your surplus is: " + (sum - selectedDrinkPrice) + "\nThe coins:");
                PrintCoins(l);
            }
            return true;
        }

        //The method returns a list with the minimum possible amount of coins (With greedy algorithm - it is optimal with these coins values).
        public List<float> GetSurplus(float surplus)
        {
            List<float> surplusCoins = new List<float>();
            for (int i = 1; i < machine.Coins.Count; i++)
            {
                while (surplus - machine.Coins[i].Value >= 0 && machine.Coins[i].Quantity > 0)
                {
                    surplus -= machine.Coins[i].Value;
                    surplusCoins.Add(machine.Coins[i].Value);
                    machine.Coins[i].Quantity--;
                }
                if (surplus == 0)
                    break;
            }
            return surplusCoins;
        }

        //The method saves the VendingMachin object after the changes and returns true/false according to success.
        public bool UpdateMachineStock()
        {
            try
            {
                string json = JsonConvert.SerializeObject(machine);
                System.IO.File.WriteAllText(@"../../../vendingMachine.json", json);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


    }

}
