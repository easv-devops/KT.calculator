namespace test;

public class Tests
{

    [Test]
    public void TestAdd()
    {
        Calculator calculator = new Calculator();
        Assert.That(calculator.Add(5,5), Is.EqualTo(10));
    }

    [Test]
    public void TestSubtract()
    {
        Calculator calculator = new Calculator();
        Assert.That(calculator.Subtract(6, 5), Is.EqualTo(1));
    }

    [Test]
    public void TestMultiply()
    {
        Calculator calculator = new Calculator();
        Assert.That(calculator.Multiply(2,3), Is.EqualTo(6));
    }

    [Test]
    public void TestDivide()
    {
        Calculator calculator = new Calculator();
        Assert.That(calculator.Divide(12, 3), Is.EqualTo(4));
    }
}