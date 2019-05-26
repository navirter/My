using Microsoft.VisualStudio.TestTools.UnitTesting;
using My;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Testing
{
    [TestClass()]
    public class CopierTesting
    {
        class Dude
        {
            public string name = "";
            public int age = 0;
            public Sex sex = Sex.Male;
            public enum Sex
            {
                Male,
                Female
            }
            public Dude(string name, int age, Sex sex)
            {
                this.name = name;
                this.age = age;
                this.sex = sex;
            }
            public Dude() { }
            public override bool Equals(object obj)
            {
                Dude other = obj as Dude;
                if (name != other.name)
                    return false;
                if (age != other.age)
                    return false;
                if (sex != other.sex)
                    return false;
                return true;
            }
        }

        class Parent : Dude
        {
            public int kidsAmount = 0;

            public Parent() { }
            public Parent(string name, int age, Sex sex, int kids)
                : base(name, age, sex)
            {
                this.kidsAmount = kids;
            }
            public override bool Equals(object obj)
            {
                Parent p = obj as Parent;
                if (!base.Equals(p))
                    return false;
                return p.kidsAmount == kidsAmount;
            }
        }

        [TestMethod()]
        public void copyFieldsTesting_SimpleClass()
        {
            Dude d = new Dude("Valera", 22, Dude.Sex.Male);
            Dude res = new Dude();
            Copier.CopyUsingSerializer(d, res);
            Assert.AreEqual(d, res);
        }

        [TestMethod()]
        public void copyFieldsTesting_Null()
        {
            try
            {
                Dude d = null;
                Dude res = new Dude();
                Copier.CopyUsingSerializer(d, res);
                Assert.Fail();
            }
            catch
            { Assert.IsTrue(true); }
        }

        [TestMethod()]
        public void copyFieldsTesting_Inheritation()
        {
            Dude d = new Dude("Valera", 22, Dude.Sex.Male);
            Parent res = new Parent("Vitaliy", 44, Dude.Sex.Female, 4);//subclass of Dude
            Copier.CopyUsingSerializer(d, res);
            bool match = d.name == res.name && d.age == res.age && d.sex == res.sex;
            Assert.IsTrue(match);
        }

        [TestMethod()]
        public void copyFieldsTesting_ReverseInheritation()
        {
            Parent d = new Parent("Vitaliy", 44, Dude.Sex.Female, 4);//subclass of Dude
            Dude res = new Dude("Valera", 22, Dude.Sex.Male);
            Copier.CopyUsingSerializer(d, res);
            bool match = d.name == res.name && d.age == res.age && d.sex == res.sex;
            Assert.IsTrue(match);
        }

        [TestMethod()]
        public void copyFieldsTesting_BrakeReference()
        {
            try
            {
                Dude d = new Dude();
                Dude res = new Dude();
                Copier.CopyUsingSerializer(d, res);
                d.age = res.age + 1;
                Assert.AreNotEqual(d.age, res.age);
            }
            catch (Exception e)
            { Assert.Fail(e.Message); }
        }

        [TestMethod()]
        public void copyFieldsTesting_BrakeReferenceInList()
        {
            try
            {
                List<Dude> dudes = new List<Dude>() { new Dude() };
                List<Dude> res = new List<Dude>();
                Copier.CopyUsingSerializer(dudes, res, false);
                dudes[0].age = 2;
                Assert.AreNotEqual(dudes[0].age, res[0].age);
            }
            catch (NotSupportedException e)
            {
                
            }
            catch (Exception e)
            { Assert.Fail(e.Message); }
        }
    }
}