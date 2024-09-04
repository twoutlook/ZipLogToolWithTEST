using NUnit.Framework;
using System.IO;

namespace ZipLogTool.Tests
{
    [TestFixture]
    public class ZipLogToolInitTests
    {
        private ZipLogTestCase _zipLogTestCase;
        private string _testFolderPath;

        [SetUp]
        public void Setup()
        {
            _zipLogTestCase = new ZipLogTestCase(1);
            //_testFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "TESTCASE001");
            _testFolderPath = "D:\\LAB\\TESTCASE001";

            // Clean up any previous test folders
            if (Directory.Exists(_testFolderPath))
            {
                Directory.Delete(_testFolderPath, true);
            }
        }

        [Test]
        public void InitTestCase001_CreatesFolders()
        {
            // Act
            _zipLogTestCase.InitTestCase001();

            // Assert - Verify that the folder was created
            Assert.That(Directory.Exists(_testFolderPath), Is.True, "TESTCASE001 folder should be created");
            
            // Add more assertions to verify other folders or files as needed
        }

        [Test]
        public void InitTestCase002_CreatesFiles()
        {
            // Arrange - Run the first initialization to create the folders
            _zipLogTestCase.InitTestCase001();
            string testFilePath = Path.Combine(_testFolderPath, "testfile.txt");

            // Act
            _zipLogTestCase.InitTestCase002();

            // Assert - Verify that the file was created
            Assert.That(File.Exists(testFilePath), Is.True, "testfile.txt should be created in TESTCASE001");
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up test folders after test execution
            if (Directory.Exists(_testFolderPath))
            {
                Directory.Delete(_testFolderPath, true);
            }
        }
    }
}
