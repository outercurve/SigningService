using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CmdletTesting.Properties;
using Moq;
using Outercurve.DTOs;
using Outercurve.DTOs.Services.Azure;
using Outercurve.ToolsLib.Services;
using Xunit;

namespace CmdletTesting
{
    public class Testing
    {
        [Fact]
        public void TestTheDeleting()
        {
            var s = new Mock<IAzureService>();
            
            var containerDictionary = SetupContainers();
            s.Setup(i => i.Containers).Returns(containerDictionary.Keys.Select(k => k.Object));

            var fileService = new AzureFilesService(s.Object);
            fileService.DeleteFiles();
            VerifyProperFilesHaveBeenDeleted(containerDictionary);
        }

        private void VerifyProperFilesHaveBeenDeleted(Dictionary<Mock<IAzureContainer>, bool> containers)
        {
            foreach (var c in containers)
            {
                KeyValuePair<Mock<IAzureContainer>, bool> c1 = c;
              
                Assert.DoesNotThrow(() => c1.Key.Verify(cont => cont.Delete(), (c1.Value ? Times.Once(): Times.Never())));
                
            }
        }


        private Dictionary<Mock<IAzureContainer>,bool> SetupContainers()
        {
            var xdoc = XDocument.Parse(Resources.SampleDataForDeleting);
            var output = new Dictionary<Mock<IAzureContainer>, bool>();

            foreach (var con in xdoc.Elements("sample").First().Elements("container"))
            {
                var name = con.Attribute("name").Value;
                var deleted = bool.Parse(con.Attribute("shouldBeDeleted").Value);
                var policies = con.Elements("policy").Select(CreatePolicy).ToDictionary(kv => kv.Key, kv => kv.Value);
                var azureContainer = new Mock<IAzureContainer> {CallBase = true};
                azureContainer.Setup(c => c.Delete());
                azureContainer.Setup(c => c.GetSharedAccessPolicies()).Returns(policies);
                azureContainer.Setup(c => c.Uri).Returns(() => new Uri( "http://not_set.com"));
                
                azureContainer.SetupGet(c => c.Name).Returns(name);
                
                output.Add(azureContainer, deleted);
            }



            return output;

        }

        private KeyValuePair<string, IAzureSharedAccessPolicy> CreatePolicy(XElement elem)
        {
            var mock = new Mock<IAzureSharedAccessPolicy>();
            var newDateTime = DateTimeOffset.Now;
            var addAttrib = elem.Attribute("add");
            if (addAttrib != null)
            {
                newDateTime = newDateTime.AddHours(Double.Parse(addAttrib.Value));
            }
            else
            {
                var minus = elem.Attribute("minus");
                var span = new TimeSpan(int.Parse(minus.Value), 0, 0);
                newDateTime = newDateTime.Subtract(span);
            }

            mock.Setup(p => p.SharedAccessExpiry).Returns(newDateTime);
            return new KeyValuePair<string, IAzureSharedAccessPolicy>(elem.Attribute("name").Value, mock.Object);
        }
        


        [Fact]
        public void TestBinary()
        {
            using (var s = new MemoryStream(Properties.Resources.ServiceStack_OrmLite_SqlServer))
            {
                Assert.True(FileSensing.IsItAPEFile(s));
            }

            using (var s = new MemoryStream(Properties.Resources.TestVsix))
            {
                Assert.False(FileSensing.IsItAPEFile(s));
            }
        }


        [Fact]
        public void TestZip()
        {
            using (var s = new MemoryStream(Resources.TestVsix))
            {
                Assert.True(FileSensing.IsItAZipFile(s));
            }

            using (var s = new MemoryStream(Properties.Resources.ServiceStack_OrmLite_SqlServer))
            {
                Assert.False(FileSensing.IsItAZipFile(s));
            }
        }
        
    }
}
