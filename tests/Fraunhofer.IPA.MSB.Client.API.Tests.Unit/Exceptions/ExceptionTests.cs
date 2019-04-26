﻿// <copyright file="ExceptionTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
// Copyright 2019 Fraunhofer Institute for Manufacturing Engineering and Automation IPA
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Fraunhofer.IPA.MSB.Client.API.Tests.Unit.Exceptions
{
    using Fraunhofer.IPA.MSB.Client.API.Exceptions;
    using Xunit;

    public class ExceptionTests
    {
        public ExceptionTests()
        {
        }

        public class Constructors : ExceptionTests
        {
            [Fact]
            public void EventNotExistExpcetion()
            {
                string expectedMessage = "ErrorMessage";
                var exception = new EventNotExistException(expectedMessage);
                Assert.Equal(expectedMessage, exception.Message);
            }

            [Fact]
            public void InvalidMsbMethodDefinitionException()
            {
                string expectedMessage = "ErrorMessage";
                var exception = new InvalidMsbMethodDefinitionException(expectedMessage);
                Assert.Equal(expectedMessage, exception.Message);
            }

            [Fact]
            public void ResponseEventNotFoundException()
            {
                string expectedMessage = "ErrorMessage";
                var exception = new ResponseEventNotFoundException(expectedMessage);
                Assert.Equal(expectedMessage, exception.Message);
            }

            [Fact]
            public void ServiceNotRegisteredException()
            {
                string expectedMessage = "ErrorMessage";
                var exception = new ServiceNotRegisteredException(expectedMessage);
                Assert.Equal(expectedMessage, exception.Message);
            }
        }
    }
}
