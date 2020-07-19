using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Xunit;

namespace Doublel.ReflexionExtensions.test
{
    public class AttributeTests
    {
        [Fact]
        public void GenericHasAttribute_ReturnsTrueWhenAttributeIsPresent() => 
            TestClass.Instance.GetProperty("FirstName").HasAttribute<ForeignKeyAttribute>().Should().BeTrue();

        [Fact]
        public void GenericHasAttribute_ReturnsFalseWhenAttributeIsNotPresent() =>
            TestClass.Instance.GetProperty("FirstName").HasAttribute<TableAttribute>().Should().BeFalse();

        [Fact]
        public void GenericHasAttribute_WorksWhenThereAreMultipleAttributes()
        {
            var property = TestClass.Instance.GetProperty("FirstName");
            property.HasAttribute<ForeignKeyAttribute>().Should().BeTrue();
            property.HasAttribute<KeyAttribute>().Should().BeTrue();
            property.HasAttribute<DataMemberAttribute>().Should().BeTrue();
            property.HasAttribute<TableAttribute>().Should().BeFalse();
        }

        [Fact]
        public void GenericGetAttributeWorksAsExpected() =>
            TestClass.Instance.GetProperty("FirstName").GetAttribute<ForeignKeyAttribute>().Name.Should().Be("Test");

        [Fact]
        public void GenericGetAttributeValueWorksAsExpected() =>
            TestClass.Instance.GetProperty("FirstName").GetAttributeValue<ForeignKeyAttribute, string>(x => x.Name).Should().Be("Test");
        
    }

    public class TestClass
    {
        [ForeignKey("Test")]
        [Key]
        [DataMember]
        public int FirstName { get; set; }
        public static TestClass Instance => new TestClass();
    }
}
