// See https://aka.ms/new-console-template for more information
using BogusTest;


Console.WriteLine("Hello, World!");

DataGeneratorTest dataGeneratorTest = new DataGeneratorTest();

var fakeTrade = dataGeneratorTest.GenerateFakeTrade();

Console.WriteLine(fakeTrade);

Console.WriteLine("-----------------------");

var dataGeneratorTest1 = new DataGeneratorTest();

dataGeneratorTest1.LoadFakeData();
