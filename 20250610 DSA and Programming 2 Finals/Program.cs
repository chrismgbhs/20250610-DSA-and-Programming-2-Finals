using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _20250610_DSA_and_Programming_2_Finals
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double accNum;
            int accPin;
            int transactionInput;
            int accountHolderC = 0;
            int billPC = 0;

            bool accFound = false;
            bool startAgain = true;

            string[] accDetails = {""};
            string[] accounts;

            List<string> transactionList = new List<string> { "1. Withdrawal", "2. Deposit", "3. Balance"};
            List<string> accountsHolder = new List<string>();

            // VARIABLES FOR WITHDRAWAL
            int withdrawalAmount;
            int machineBalance;
            int billsLine;
            int multiplier;
            int wOutput;
            string[] billP;
            bool doWithdraw = true;


            // ENTERING OF ACCOUNT NUMBER
            while (true) 
            {
                Console.Write("Please enter your account number: ");
                if (double.TryParse(Console.ReadLine(), out accNum))
                {
                    break;
                }

                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }

            Console.WriteLine();

            if (File.Exists("accounts.csv"))
            {
                accounts = File.ReadAllLines("accounts.csv");

                foreach (string account in accounts)
                {
                    accountsHolder.Add(account);
                }

                for (accountHolderC = 0; accountHolderC < accountsHolder.Count; accountHolderC++)
                {
                    string[] accountDetails = accountsHolder[accountHolderC].Split(',');

                    if (double.Parse(accountDetails[0]) == accNum)
                    {
                        accFound = true;
                        accDetails = accountDetails;
                        break;
                    }
                }

                if (accFound)
                {
                    while (true)
                    {
                        Console.Write("Please enter your pin: ");
                        if (int.TryParse(Console.ReadLine(), out accPin))
                        {
                            break;
                        }
                    }

                    if (double.Parse(accDetails[1]) == accPin)
                    {
                        Console.Clear();

                        Console.WriteLine("Welcome " + accDetails[0] + "! Your balance is PHP" + accDetails[2] + ".");
                        Console.WriteLine("Thank you for using our service.");
                        Console.WriteLine();
                        File.Create($"Transactions_{accDetails[0]}.csv").Close(); // Create a transaction file for the account
                        
                        foreach (string transaction in transactionList)
                        {
                            Console.WriteLine(transaction);
                        }

                        while (true)
                        {
                            Console.Write("Please select the transaction mode that you want to proceed: ");
                            if (int.TryParse(Console.ReadLine(), out transactionInput))
                            {
                                break;
                            }

                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a number.");
                            }
                        }

                        Console.Clear();

                        switch (transactionInput)
                        {
                            case 1: // Withdrawal
                                machineBalance = 0;
                                while (true)
                                {
                                    Console.Write("Please enter the amount you want to withdraw: PHP ");
                                    if (int.TryParse(Console.ReadLine(), out withdrawalAmount))
                                    {
                                        if (withdrawalAmount <= int.Parse(accDetails[2]) && withdrawalAmount > 0)
                                        {
                                            break;
                                        }

                                        else
                                        {
                                            Console.WriteLine("Insufficient balance or invalid amount. Please try again.");
                                        }
                                    }

                                    else
                                    {
                                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                                    }
                                }

                                // SORT BILLS.CSV FILE
                                List<string> bills = new List<string>();
                                billP = File.ReadAllLines("bills.csv");
                                foreach (string bill in billP)
                                {
                                    bills.Add(bill);
                                }

                                while (billPC < billP.Length - 1)
                                {
                                    billPC = 1;

                                    while (true)
                                    {
                                        if (int.Parse(billP[billPC].Split(',')[0]) < int.Parse(billP[billPC - 1].Split(',')[0]))
                                        {
                                            bills.Insert(billPC - 1, billP[billPC]);
                                            bills.RemoveAt(billPC + 1);
                                            break;
                                        }

                                        else
                                        {
                                            billPC++;
                                        }
                                    }
                                    
                                }
                                
                                File.Delete("bills.csv");
                                File.WriteAllLines("bills.csv", bills);

                                foreach (string bill in bills)
                                {
                                    string[] billDetails = bill.Split(',');
                                    machineBalance += int.Parse(billDetails[0]) * int.Parse(billDetails[1]);
                                }

                                while (doWithdraw)
                                {
                                    billsLine = bills.Count();
                                    billsLine--;
                                    multiplier = 0;

                                    while (true)
                                    {
                                        if (billsLine == -1)
                                        {
                                            doWithdraw = false;
                                            break;
                                        }

                                        else
                                        {

                                            if (int.Parse(bills[billsLine].Split(',')[0]) <= withdrawalAmount)
                                            {
                                                multiplier++;
                                                wOutput = int.Parse(bills[billsLine].Split(',')[0]) * (multiplier - 1);

                                                if (int.Parse(bills[billsLine].Split(',')[0]) * multiplier <= int.Parse(bills[billsLine].Split(',')[0]) * int.Parse(bills[billsLine].Split(',')[1]) && int.Parse(bills[billsLine].Split(',')[0]) * multiplier <= withdrawalAmount)
                                                {
                                                    wOutput = int.Parse(bills[billsLine].Split(',')[0]) * multiplier;
                                                }

                                                else
                                                {
                                                    Console.WriteLine("The ATM has dispensed PHP " + wOutput + "with " + (multiplier - 1) + "x " + bills[billsLine].Split(',')[0] + " bills.");
                                                    withdrawalAmount -= wOutput;
                                                    accDetails[2] = (int.Parse(accDetails[2]) - wOutput).ToString();
                                                    accountsHolder[accountHolderC] = $"{accDetails[0]},{accDetails[1]},{accDetails[2]}";
                                                    File.WriteAllLines("accounts.csv", accountsHolder);
                                                    bills[billsLine] = $"{bills[billsLine].Split(',')[0]},{int.Parse(bills[billsLine].Split(',')[1]) - (multiplier - 1)}";
                                                    File.AppendAllText($"Transactions_{accDetails[0]}.csv", $"Withdrawal,{wOutput},{DateTime.Now}\n");
                                                    File.WriteAllLines("bills.csv", bills);
                                                    break;
                                                }
                                            }

                                            else
                                            {
                                                billsLine--;
                                            }
                                        }
                                    }
                                }
                                



                                break;

                            case 2: // Deposit
                                Console.Write("Please enter the amount you want to deposit: ");
                                if (int.TryParse(Console.ReadLine(), out int depositAmount))
                                {
                                    accDetails[3] = (int.Parse(accDetails[3]) + depositAmount).ToString();
                                    Console.WriteLine("Deposit successful! Your new balance is PHP" + accDetails[3] + ".");
                                    File.AppendAllText($"Transactions_{accDetails[0]}.csv", $"Deposit,{depositAmount},{DateTime.Now}\n");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid amount.");
                                }
                                break;
                            case 3: // Balance Inquiry
                                Console.WriteLine("Your current balance is PHP" + accDetails[3] + ".");
                                break;
                            default:
                                Console.WriteLine("Invalid transaction mode selected.");
                                break;
                        }
                    }

                    else
                    {
                        Console.WriteLine("Incorrect pin. Please try again.");
                    }


                }

                else
                {
                    Console.WriteLine("Account not found. Thank you.");
                }
            }

            else
            {
                Console.WriteLine("No accounts found. Please contact administrator.");
            }
        }
    }
}
