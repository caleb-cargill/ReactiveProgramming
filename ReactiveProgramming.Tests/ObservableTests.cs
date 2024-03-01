using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProgramming.Tests
{
    [TestFixture]
    public class ObservableTests
    {
        [Test]
        public void Test_SimpleCollectionObservable()
        {
            // Arrange
            var subjects = DbContext.GetSubjects();
            var members = subjects.SelectMany(s => s.Members).ToList();
            var calculations = DbContext.GetCalculations();
            var results = new List<string>();

            // Act
            var subjectStream = subjects.ToObservable();
            subjectStream.Subscribe(s => results.Add(s.Name));

            var memberStream = subjectStream.SelectMany(s => s.Members);
            memberStream.Subscribe(m => results.Add(m.Name));

            var calculationStream = memberStream.Select(m => calculations.FirstOrDefault(c => c.Updates == m.Name)).Where(c => c != null);
            calculationStream.Subscribe(c => results.Add(c.Name));

            // Assert
            Assert.IsTrue(results.Any());
        }

        [Test]
        public void Test_SimplePropertyChangedObservable()
        {
            // Arrange
            var cube = Source.GetCube();
            var results = new List<string>();

            // Act
            var propertyChangedEvents =
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => cube.PropertyChanged += h,
                    h => cube.PropertyChanged -= h);
            var propertyChangedObserver =
                propertyChangedEvents.Subscribe(e => results.Add("Property Changed: " + e.EventArgs.PropertyName));

            cube.Height = 5;
            cube.Volume = 125;
            cube.Height = 10;
            cube.Volume = 1000;

            propertyChangedObserver.Dispose();

            cube.Height = 20;
            cube.Volume = 8000;

            // Assert
            Assert.IsTrue(results.Any());
        }
    }
}
