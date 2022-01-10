using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seeking_Alpha_test___vending_machin;
using System.Collections.Generic;

namespace VendingMachineTests
{
    [TestClass]
    public class VendingMachineTests
    {
        [TestMethod]
        //Testing method LoadMachine() by inserting an invalid path.
        public void CheckLoadMachine_WithInvalidInput()
        {
            VendingMachine machine = new VendingMachine();
            bool actual = machine.LoadMachine("../");
            Assert.IsFalse(actual);
        }

        [TestMethod]
        //Testing method GetMassegeForSelectedDrink()  with invalid input.
        public void CheckInputValidation_WithInvalidInput()
        {
            VendingMachine machine = new VendingMachine();
            string actual = machine.GetMassegeForSelectedDrink("h");
            Assert.AreEqual("You have a typo, try again...", actual);
        }

        [TestMethod]
        //Testing the method UpdateMachineStock().
        public void CheckSaving()
        {
            VendingMachine machine = new VendingMachine();
            bool actual = machine.UpdateMachineStock();
            Assert.AreEqual(true, actual);
        }
        


    }
}
