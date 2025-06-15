using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;

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

            List<string> transactionList = new List<string> { "1. Withdrawal", "2. Deposit", "3. Balance", "4. Exit"};
            List<string> accountsHolder = new List<string>();

            // VARIABLES FOR WITHDRAWAL
            int withdrawalAmount;
            int machineBalance;
            int billsLine;
            int multiplier;
            int wOutput;
            int billLength;
            int finalWithdrawAmount;
            string[] billP;
            bool doWithdraw;

            // VARIABLES FOR DEPOSIT
            int billDenominationChoice;
            int billDenomination;
            int billQuantity;
            int depositAmount;


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

                    while (startAgain)
                    {
                        if (double.Parse(accDetails[1]) == accPin)
                        {
                            Console.Clear();

                            Console.WriteLine("Welcome " + accDetails[0] + "! Your balance is PHP" + accDetails[2] + ".");
                            Console.WriteLine("Thank you for using our service.");
                            Console.WriteLine();

                            if (!File.Exists($"{accDetails[0]}_Transactions.csv"))
                            {
                                File.Create($"{accDetails[0]}_Transactions.csv").Close(); // Create a transaction file for the account
                            }

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
                                    finalWithdrawAmount = 0;
                                    if (accDetails[2] == "0")
                                    {
                                        Console.WriteLine("You have no balance to withdraw. Please deposit first.");
                                    }

                                    else
                                    {
                                        doWithdraw = true;
                                        machineBalance = 0;
                                        while (true)
                                        {
                                            Console.Write("Please enter the amount you want to withdraw: PHP");
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
                                            //Console.WriteLine($"*{bill}* has been added to bills list");
                                            bills.Add(bill.Trim());
                                        }

                                        billLength = bills.Count;
                                        //Console.WriteLine($"Your bill length is {billLength}");

                                        for (int billCounter = 0; billCounter < billLength - 1; billCounter++)
                                        {
                                            //Console.WriteLine($"Your bill length is {billLength}");
                                            //Console.WriteLine($"Checking bill {billCounter}...");
                                            if (int.Parse(bills[billCounter].Split(',')[1]) == 0)
                                            {
                                                //Console.WriteLine($"Bill {bills[billCounter]} has been removed from the list.");
                                                bills.RemoveAt(billCounter);
                                                billLength = bills.Count;
                                            }
                                        }

                                        while (billPC < billLength - 1)
                                        {
                                            billPC = 1;

                                            while (true)
                                            {
                                                if (int.Parse(bills[billPC].Split(',')[0]) < int.Parse(bills[billPC - 1].Split(',')[0]))
                                                {
                                                    bills.Insert(billPC - 1, billP[billPC]);
                                                    bills.RemoveAt(billPC + 1);
                                                    //Console.WriteLine($"{billP[billPC]} has been moved to position {billPC}.");
                                                    break;
                                                }

                                                else if (billPC == billLength - 1)
                                                {
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

                                        if (withdrawalAmount > machineBalance)
                                        {
                                            Console.WriteLine("The ATM does not have enough balance to dispense the requested amount. Please try again later.");
                                            doWithdraw = false;
                                            break;
                                        }

                                        if (withdrawalAmount < int.Parse(bills[0].Split(',')[0]))
                                        {
                                            Console.WriteLine("The ATM will not dispense bills less than PHP " + bills[0].Split(',')[0] + ".");
                                            Console.WriteLine("You may try to withdraw again with a different amount.");
                                            doWithdraw = false;
                                            break;
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
                                                            if (!File.Exists($"Master Transactions List.csv"))
                                                            {
                                                                File.Create("Master Transactions List.csv").Close();
                                                                File.AppendAllText("Master Transactions List.csv", "Account Number,Transaction Type,Bill,Quantity,Date and Time\n");
                                                            }

                                                            File.AppendAllText("Master Transactions List.csv", $"{accDetails[0]},Withdrawal,{int.Parse(bills[billsLine].Split(',')[0])},{multiplier - 1},{DateTime.Now}\n");

                                                            Console.WriteLine("The ATM has dispensed PHP " + wOutput + " with " + (multiplier - 1) + "x " + bills[billsLine].Split(',')[0] + " bills.");
                                                            withdrawalAmount -= wOutput;
                                                            finalWithdrawAmount += wOutput;
                                                            accDetails[2] = (int.Parse(accDetails[2]) - wOutput).ToString();
                                                            accountsHolder[accountHolderC] = $"{accDetails[0]},{accDetails[1]},{accDetails[2]}";
                                                            File.WriteAllLines("accounts.csv", accountsHolder);
                                                            bills[billsLine] = $"{bills[billsLine].Split(',')[0]},{int.Parse(bills[billsLine].Split(',')[1]) - (multiplier - 1)}";
                                                            File.WriteAllLines("bills.csv", bills);

                                                            if (withdrawalAmount < int.Parse(bills[0].Split(',')[0]))
                                                            {
                                                                Console.WriteLine("Your remaining withdrawal amount is PHP " + withdrawalAmount + ".");

                                                                if (withdrawalAmount > 0)
                                                                {
                                                                    Console.WriteLine("Please take note that the ATM will not dispense bills less than PHP " + bills[0].Split(',')[0] + ".");
                                                                    Console.WriteLine("You may try to withdraw again with a different amount.");
                                                                }
                                                            }

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

                                        if (finalWithdrawAmount > 0)
                                        {
                                            File.AppendAllText($"{accDetails[0]}_Transactions.csv", $"Withdrawal,{finalWithdrawAmount},{DateTime.Now}\n");
                                        }
                                    }
                                   
                                    break;

                                case 2: // Deposit
                                    
                                    while (true)
                                    {
                                        billDenomination = 0;
                                        depositAmount = 0;
                                        Console.WriteLine("1. 20 PHP\n2. 50 PHP\n3. 100 PHP\n4. 200 PHP\n5. 500 PHP\n6. 1000 PHP");

                                        while (true)
                                        {
                                            Console.Write("Please select the bill denomination you want to deposit: ");
                                            if (int.TryParse(Console.ReadLine(), out billDenominationChoice) && billDenominationChoice > 0 && billDenominationChoice < 7)
                                            {
                                                break;
                                            }
                                        }

                                        switch (billDenominationChoice)
                                        {
                                            case 1:
                                                billDenomination = 20;
                                                break;

                                            case 2:
                                                billDenomination = 50;
                                                break;

                                            case 3:
                                                billDenomination = 100;
                                                break;

                                            case 4:
                                                billDenomination = 200;
                                                break;

                                            case 5:
                                                billDenomination = 500;
                                                break;

                                            case 6:
                                                billDenomination = 1000;
                                                break;
                                        }



                                        while (true)
                                        {
                                            Console.Write("Please enter the quantity of PHP" + billDenomination + " bills you want to deposit: ");
                                            if (int.TryParse(Console.ReadLine(), out billQuantity) && billQuantity > 0)
                                            {
                                                depositAmount = billDenomination * billQuantity;

                                                if (!File.Exists($"Master Transactions List.csv"))
                                                {
                                                    File.Create("Master Transactions List.csv").Close();
                                                    File.AppendAllText("Master Transactions List.csv", "Account Number,Transaction Type,Bill,Quantity,Date and Time\n");
                                                }

                                                File.AppendAllText("Master Transactions List.csv", $"{accDetails[0]},Deposit,{billDenomination},{billQuantity},{DateTime.Now}\n");
                                                File.AppendAllText($"{accDetails[0]}_Transactions.csv", $"Deposit,{depositAmount},{DateTime.Now}\n");
                                                Console.WriteLine($"You have deposited PHP{depositAmount}.");
                                                accDetails[2] = (int.Parse(accDetails[2]) + depositAmount).ToString();
                                                accountsHolder[accountHolderC] = $"{accDetails[0]},{accDetails[1]},{accDetails[2]}";
                                                File.WriteAllLines("accounts.csv", accountsHolder);

                                                List<string> bills = new List<string>(File.ReadAllLines("bills.csv"));

                                                for (int billLineCounter = 0; billLineCounter < bills.Count; billLineCounter++)
                                                {
                                                    if (bills[billLineCounter].Split(',')[0] == billDenomination.ToString())
                                                    {
                                                        bills[billLineCounter] = $"{billDenomination},{int.Parse(bills[billLineCounter].Split(',')[1]) + billQuantity}";
                                                        break;
                                                    }
                                                }

                                                File.WriteAllLines("bills.csv", bills);

                                                break;
                                            }

                                            else
                                            {
                                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                            }
                                        }

                                        Console.WriteLine("Do you want to deposit more bills? Enter 'Y' to deposit more, otherwise will go back to main menu.");
                                        if (Console.ReadLine() != "Y" && Console.ReadLine() != "y")
                                        {
                                            break;
                                        }
                                    }
                                break;
                                case 3: // Balance Inquiry
                                    Console.WriteLine("Your current balance is PHP" + accDetails[2] + ".");
                                    break;
                                case 4: // Exit
                                    Console.WriteLine("Thank you for using our service. Goodbye!");
                                    startAgain = false;
                                    break;

                                default:
                                    Console.WriteLine("Invalid transaction mode selected.");
                                    break;  
                            }

                            Console.WriteLine();
                            Console.WriteLine("Press anything to continue...");
                            Console.ReadKey();
                        }

                        else
                        {
                            Console.WriteLine("Incorrect pin. Please try again.");
                        }
                    }

                    if (File.Exists($"{accDetails[0]}_Transactions.csv"))
                    {
                        Console.Clear();
                        Console.WriteLine("TRANSACTION RECEIPT");
                        Console.WriteLine("ACCOUNT NUMBER\t " + accDetails[0]);
                        Console.WriteLine("---------------------------------------------");
                        Console.WriteLine();
                        Console.WriteLine("TRANSACTION TYPE\t\tAMOUNT\t\tTIME");

                        foreach (string line in File.ReadAllLines($"{accDetails[0]}_Transactions.csv")) // Skip the header line
                        {
                            string[] transactionDetails = line.Split(',');
                            Console.WriteLine($"{transactionDetails[0]}\t\t\t{transactionDetails[1]}\t\t{transactionDetails[2]}");
                        }

                        File.Delete($"{accDetails[0]}_Transactions.csv");

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
