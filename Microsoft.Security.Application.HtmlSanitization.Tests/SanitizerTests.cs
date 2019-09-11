namespace Microsoft.Security.Application.HtmlSanitization.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the Sanitizer.
    /// </summary>
    [TestClass()]
    public class SanitizerTests
    {
        /// <summary>
        /// Style attributes should be removed from elements in HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleableAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><p style=\"\"></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<p></p>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style attributes should be removed from elements in HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_StyleableAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<p style=\"\"></p>";
            string expected = "\r\n<p></p>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style attributes should be removed from elements in HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleableAttributeOnTagShouldBeRemovedLeavingOtherTags()
        {
            // Arrange
            string input = "<html><head></head><body><a href=\"\" style=\"\"></a></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<a href=\"\"></a>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style attributes should be removed from elements in HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_StyleableAttributeOnTagShouldBeRemovedLeavingOtherTags()
        {
            // Arrange
            string input = "<a href=\"\" style=\"\"></a>";
            string expected = "<a href=\"\"></a>";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Style tags should be removed from the head element.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleTagInHeaderShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head><style></style></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Style tags should be removed from the head element.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleTagInHeaderShouldBeRemovedAdvanced()
        {
            // Arrange
            string input = "<html><head><style>* {color:rgb(0,0,0)\\-o-link:'data:text/html,%3c%73%63%72%69%70%74%3e%61%6c%65%72%74%28%31%29%3c%2f%73%63%72%69%70%74%3e';color:rgb(x)\\-o-link-source:current;}</style></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style tags should be removed from the body.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleTagInBodyShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><style></style></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_OnClickAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><p onclick=\"\"></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<p></p>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_OnClickAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<p onclick=\"\"></p>";
            string expected = "\r\n<p></p>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_OnMouseOverAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><p onmouseover=\"\"></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<p></p>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_OnMouseOverAttributeOnTagShouldBeRemoved()
        {
            // Arrange
            string input = "<p onmouseover=\"\"></p>";
            string expected = "\r\n<p></p>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_OnLoadAttributeOnBodyTagShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body onload=\"\"></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ErrorAttributeShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><img src=\"\" onerror=\"XSS\" /></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img src=\"\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Scriptable attributes should be removed from HTML.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ErrorAttributeShouldBeRemoved()
        {
            // Arrange
            string input = "<img src=\"\" onerror=\"XSS\" />";
            string expected = "<img src=\"\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Script tags should be removed from the head element.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptTagInHeaderShouldBeRemoved()
        {
            // Arrange
            string input = @"<html><head><script></script></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Script tags should be removed from the body.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptTagInBodyShouldBeRemoved()
        {
            // Arrange
            string input = @"<html><head></head><body><script></script></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_StyleTagShouldBeRemovedUserTwo()
        {
            // Arrange
            string input = "<div style=\"font-family:Foo,Bar\\,'a\\a';font-family:';color:expression(alert(1));y'\">aaa</div>";
            string expected = "<html>\r\n<body>\r\n<div>aaa</div>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Style tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_StyleTagShouldBeRemovedUserTwo()
        {
            // Arrange
            string input = "<div style=\"font-family:Foo,Bar\\,'a\\a';font-family:';color:expression(alert(1));y'\">aaa</div>";
            string expected = "\r\n<div>aaa</div>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Break tags should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/18339</example>
        [TestMethod()]
        public void GetSafeHtml_BreakTagsShouldNotBeRemoved()
        {
            // Arrange
            string input = @"<html><head></head><body><br></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<br>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Break tags should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/18339</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_BreakTagsShouldNotBeRemoved()
        {
            // Arrange
            string input = "<br>";
            string expected = "<br>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// New lines should not be added.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/17733</example>
        [TestMethod()]
        public void GetSafeHtml_NewLinesShouldNotBeAdded()
        {
            // autoNewLines in HtmlWriter.cs

            // Arrange
            string input = "<html><head></head><body>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nunc tellus, consectetur eget blandit euismod, pharetra a libero. In pretium, sem sed mollis hendrerit, libero metus condimentum tellus, eget adipiscing odio ligula at velit. Nulla luctus nisl quis sem venenatis ut suscipit mauris posuere.</body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\nLorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nunc tellus, consectetur eget blandit euismod, pharetra a libero. In pretium, sem sed mollis hendrerit, libero metus condimentum tellus, eget adipiscing odio ligula at velit. Nulla luctus nisl quis sem venenatis ut suscipit mauris posuere.\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// New lines should not be added.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/17733</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_NewLinesShouldNotBeAdded()
        {
            // autoNewLines in HtmlWriter.cs

            // Arrange
            string input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nunc tellus, consectetur eget blandit euismod, pharetra a libero. In pretium, sem sed mollis hendrerit, libero metus condimentum tellus, eget adipiscing odio ligula at velit. Nulla luctus nisl quis sem venenatis ut suscipit mauris posuere.";
            string expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nunc tellus, consectetur eget blandit euismod, pharetra a libero. In pretium, sem sed mollis hendrerit, libero metus condimentum tellus, eget adipiscing odio ligula at velit. Nulla luctus nisl quis sem venenatis ut suscipit mauris posuere.";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Div tags should not be added.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/15926</example>
        [TestMethod()]
        public void GetSafeHtml_DivTagsShouldNotBeAdded()
        {
            // Arrange
            string input = "<html><head></head><body><input type=\"text\" /></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<input type=\"text\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Div tags should not be added.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/15926</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_DivTagsShouldNotBeAdded()
        {
            // Arrange
            string input = "<input type=\"text\" />";
            string expected = "<input type=\"text\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Don't add a carriage return after whitespace at the end of a fragment.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/15451</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_DoNotAddCarriageReturnAfterEndingWhitespace()
        {
            // Arrange
            string input = "This brown fox ";
            string expected = "This brown fox ";
            string incorrect = "This brown fox\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(incorrect, actual);
        }

        ///<summary>
        /// Duplicate attributes should be handled appropriately.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/13099</example>
        [TestMethod()]
        public void GetSafeHtml_DuplicateAttributesShouldBeHandledAppropriately()
        {
            // Arrange
            string input = "<html><head></head><body><p id=\"\" id=\"\" style=\"\" style=\"\"></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<p id=\"\" id=\"\"></p>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Duplicate attributes should be handled appropriately.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/13099</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_DuplicateAttributesShouldBeHandledAppropriately()
        {
            // Arrange
            string input = "<p id=\"\" id=\"\" style=\"\" style=\"\"></p>";
            string expected = "\r\n<p id=\"\" id=\"\"></p>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Should not remove non-offending text.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/10875</example>
        [TestMethod()]
        public void GetSafeHtml_ShouldNotRemoveNonOffendingText()
        {
            // Arrange
            string input = "<html><head></head><body><script>alert('hi');</script>This text is removed</body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\nThis text is removed\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Should not remove non-offending text.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/10875</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_ShouldNotRemoveNonOffendingText()
        {
            // Arrange
            string input = "<script>alert('hi');</script>This text is removed";
            string expected = "This text is removed";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Child controls should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/10644</example>
        [TestMethod()]
        public void GetSafeHtml_ChildControlsShouldNotBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><a href=\"\" target=\"\"><img src=\"\" /> My Image</a></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<a href=\"\" target=\"\"><img src=\"\"> My Image</a>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Child controls should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/10644</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_ChildControlsShouldNotBeRemoved()
        {
            // Arrange
            string input = "<a href=\"\" target=\"\"><img src=\"\" /> My Image</a>";
            string expected = "<a href=\"\" target=\"\"><img src=\"\"> My Image</a>";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Non-blacklisted tags should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/17246</example>
        [TestMethod()]
        public void GetSafeHtml_NonBlacklistedTagsShouldNotBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><b>Some text</b><strong>More text</strong></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<b>Some text</b><strong>More text</strong>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Non-blacklisted tags should not be removed.
        ///</summary>
        ///<example>http://wpl.codeplex.com/workitem/17246</example>
        [TestMethod()]
        public void GetSafeHtmlFragment_NonBlacklistedTagsShouldNotBeRemoved()
        {
            // Arrange
            string input = "<b>Some text</b><strong>More text</strong>";
            string expected = "<b>Some text</b><strong>More text</strong>";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptInImageSourceShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><img src=\"javascript:alert('XSS');\"></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img src=\"\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ScriptInImageSourceShouldBeRemoved()
        {
            // Arrange
            string input = "<img src=\"javascript:alert('XSS');\">";
            string expected = "<img src=\"\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptInImageSourceShouldBeRemovedTwo()
        {
            // Arrange
            string input = "<html><head></head><body><img src=javascript:alert('XSS');></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img src=\"\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ScriptInImageSourceShouldBeRemovedTwo()
        {
            // Arrange
            string input = "<img src=javascript:alert('XSS');>";
            string expected = "<img src=\"\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptInImageSourceShouldBeRemovedThree()
        {
            // Arrange
            string input = "<html><head></head><body><img src=jav   ascript:alert('XSS');></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img src=\"jav\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in image source should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ScriptInImageSourceShouldBeRemovedThree()
        {
            // Arrange
            string input = "<img src=jav   ascript:alert('XSS');>";
            string expected = "<img src=\"jav\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in malformed image tag should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptInMalformedImageTagShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><img><script>alert(\"XSS\")</script></img></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in malformed image tag should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ScriptInMalformedImageTagShouldBeRemoved()
        {
            // Arrange
            string input = "<img><script>alert(\"XSS\")</script></img>";
            string expected = "<img>";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Source encoding should be removed from image tag.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_SourceEncodingShouldBeRemovedFromImageTag()
        {
            // Arrange
            string input = "<html><head></head><body><IMG SRC=\"jav&#x09;ascript:alert('XSS');\"></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img src=\"\">\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Source encoding should be removed from image tag.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_SourceEncodingShouldBeRemovedFromImageTag()
        {
            // Arrange
            string input = "<IMG SRC=\"jav&#x09;ascript:alert('XSS');\">";
            string expected = "<img src=\"\">";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Title tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_TitleTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head><title></title></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Title tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_TitleTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<title></title>";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Link tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_LinkTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head><link rel=javascript:alert('XSS');></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Link tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_LinkTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<link rel=javascript:alert('XSS');>";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Meta tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_MetaTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head><meta http-equiv=\"refresh\" content=\"0;url=javascript:alert('XSS');\"></head><body></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Meta tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_MetaTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<meta http-equiv=\"refresh\" content=\"0;url=javascript:alert('XSS');\">";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in Table background attribute should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ScriptInTableBackgroundAttributeShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><table background=\"javascript:alert('XSS');\"></table></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<table background=\"\">\r\n</table>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Script in Table background attribute should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ScriptInTableBackgroundAttributeShouldBeRemoved()
        {
            // Arrange
            string input = "<table background=\"javascript:alert('XSS');\"></table>";
            string expected = "\r\n<table background=\"\">\r\n</table>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Object tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_ObjectTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><object classid=clsid:ae24fdae-03c6-11d1-8b76-0080c744f389><param name=url value=javascript:alert('XSS')></object></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Object tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_ObjectTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<object classid=clsid:ae24fdae-03c6-11d1-8b76-0080c744f389><param name=url value=javascript:alert('XSS')></object>";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Embed tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_EmbedTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><embed src=\"\" AllowScriptAccess=\"always\"></embed></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Embed tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_EmbedTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<embed src=\"\" AllowScriptAccess=\"always\"></embed>";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// XML tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_XMLTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<html><head></head><body><xml id=\"xss\"></xml></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// XML tags should be removed.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_XMLTagsShouldBeRemoved()
        {
            // Arrange
            string input = "<xml id=\"xss\"></xml>";
            string expected = "";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Out of order tags should still remove scripts.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_OutOfOrderTagsShouldStillRemoveScripts()
        {
            // Arrange
            string input = "<html><head></head><body><div><p></div><p><script src=\"\" /></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<div>\r\n<p></p>\r\n</div>\r\n<p></p>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Out of order tags should still remove scripts.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_OutOfOrderTagsShouldStillRemoveScripts()
        {
            // Arrange
            string input = "<div><p></div><p><script src=\"\" /></p>";
            string expected = "\r\n<div>\r\n<p></p>\r\n</div>\r\n<p></p>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Out of order tags should still remove scripts.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtml_OutOfOrderTagsShouldStillRemoveScriptsTwo()
        {
            // Arrange
            string input = "<html><head></head><body><div><p><div><p><script src=\"\" /></p></body></html>";
            string expected = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<div>\r\n<p></p>\r\n<div>\r\n<p></p>\r\n</div>\r\n</div>\r\n</body>\r\n</html>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtml(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// Out of order tags should still remove scripts.
        ///</summary>
        [TestMethod()]
        public void GetSafeHtmlFragment_OutOfOrderTagsShouldStillRemoveScriptsTwo()
        {
            // Arrange
            string input = "<div><p><div><p><script src=\"\" /></p>";
            string expected = "\r\n<div>\r\n<p></p>\r\n<div>\r\n<p></p>\r\n</div>\r\n</div>\r\n";
            string actual;

            // Act
            actual = Sanitizer.GetSafeHtmlFragment(input);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}