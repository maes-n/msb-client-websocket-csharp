﻿// <copyright file="MsbClientSample.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace Fraunhofer.IPA.MSB.Client.Websocket.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Fraunhofer.IPA.MSB.Client.API.Attributes;
    using Fraunhofer.IPA.MSB.Client.API.Configuration;
    using Fraunhofer.IPA.MSB.Client.API.Model;
    using Fraunhofer.IPA.MSB.Client.Websocket.Sample.Functions;
    using Serilog;

    public class MsbClientSample
    {
        public const string MsbWebsocketInterfaceUrl = "ws://ws.msb.ipa.cell.vfk.fraunhofer.de";

        private MsbClient myMsbClient;
        private SmartObject mySmartobject;
        private Event myEvent;

        public MsbClientSample()
        {
            this.myMsbClient = new MsbClient(MsbWebsocketInterfaceUrl);

            // SSL properties
            this.myMsbClient.AllowSslCertificateChainErrors = true;
            this.myMsbClient.AllowSslCertificateNameMismatch = true;
            this.myMsbClient.AllowSslUnstrustedCertificate = true;

            // Reconnect properties
            this.myMsbClient.AutoReconnect = true;
            this.myMsbClient.AutoReconnectIntervalInMilliseconds = 30000;

            // Sample
            this.GenerateServiceFromApplicationPropertiesFile();
            this.AddEventWithPrimtiveTypeToSelfDescription();
            this.AddEventWithComplexTypeToSelfDescription();
            this.AddFunctionToSelfDescription();
            this.AddFunctionHandlerToSelfDescription();
            this.AddConfigurationParametersToSelfDescription();
            this.GetConfigurationParmaeters();
            this.ConnectClient();
            this.RegisterService();
            this.PublishEvent();
        }

        public static void AllInOneSample()
        {
            const string MsbWebsocketInterfaceUrl = "ws://ws.msb.ipa.cell.vfk.fraunhofer.de";

            const string MyMsbSmartObjectUuid = "5607fd6f-50ae-44e7-bd6a-9416fbb25d43";
            const string MyMsbSmartObjectName = "C# Sample SmartObject";
            const string MyMsbSmartObjectDescription = "Description of my C# sample SmartObject";
            const string MyMsbSmartObjectToken = "30e47482-c140-49a9-a79f-6f2396d8e0ab";

            const string MyMsbApplicationUuid = "f69067bc-6e8c-4479-a358-8a56f7b7a54d";
            const string MyMsbApplicationName = "C# Sample Application";
            const string MyMsbApplicationDescription = "Description of my C# sample Application";
            const string MyMsbApplicationToken = "5b6b273b-18ff-420b-bbff-5f40288c18f9";

            // Create a new MsbClient which allows SmartObjects and Applications to communicate with the MSB
            MsbClient myMsbClient = new MsbClient(MsbWebsocketInterfaceUrl);

            // Create the self description of a sample SmartObject and a sample Application
            SmartObject myMsbSmartObject = new SmartObject(MyMsbSmartObjectUuid, MyMsbSmartObjectName, MyMsbApplicationDescription, MyMsbSmartObjectToken);
            Application myMsbApplication = new Application(MyMsbApplicationUuid, MyMsbApplicationName, MyMsbSmartObjectDescription, MyMsbApplicationToken);

            // Add configuration parameters
            myMsbSmartObject.AddConfigurationParameter("sampleParameter1", new ConfigurationParameterValue(1337));
            myMsbSmartObject.AddConfigurationParameter("sampleParameter2", new ConfigurationParameterValue("SampleValue"));

            // Add events
            Event simpleEvent = new Event("SimpleEventId", "Name of simple event", "Event with simple data format", typeof(string));
            Event flatEvent = new Event("FlatEventId", "Name of flat event", "Event with flat data format", typeof(Events.SimpleEvent));
            Event complexEvent = new Event("ComplexEventId", "Name of complex event", "Event with nested data format", typeof(Events.ComplexEvent));
            Event sendInts = new Event("SendIntsId", "Send Ints", "sends two integer values", typeof(IntegerValues));
            myMsbSmartObject.AddEvent(simpleEvent);
            myMsbSmartObject.AddEvent(flatEvent);
            myMsbSmartObject.AddEvent(complexEvent);
            myMsbSmartObject.AddEvent(sendInts);
            myMsbApplication.AddEvent(simpleEvent);
            myMsbApplication.AddEvent(flatEvent);
            myMsbApplication.AddEvent(complexEvent);
            myMsbApplication.AddEvent(sendInts);


            // Add functions
            SampleFunctionHandler simpleFunctions = new SampleFunctionHandler();
            myMsbSmartObject.AddFunctionHandler(simpleFunctions);

            // Connect to the MSB and register the sample SmartObject and sample Application via the MsbClient
            myMsbClient.ConnectAsync().Wait();

            myMsbClient.RegisterAsync(myMsbSmartObject).Wait();
            myMsbClient.RegisterAsync(myMsbApplication).Wait();

            // Publish events
            while (true)
            {

            }
        }

        public class IntegerValues {
            public int val1;
            public int val2;
        }
        private void GenerateServiceFromApplicationPropertiesFile()
        {
            ApplicationProperties myApplicationProperties = ApplicationProperties.Read();
            this.mySmartobject = new SmartObject(
                myApplicationProperties.Uuid,
                myApplicationProperties.Name,
                myApplicationProperties.Description,
                myApplicationProperties.Token);
        }

        private void GenerateServiceWithoutApplicationPropertiesFile()
        {
            this.mySmartobject = new SmartObject(
                "f9f57cc2-00af-408f-9ba7-5b127e5a4822",
                "C# Sample SmartObject",
                "Description of C# Sample SmartObject",
                "98e2483b-ca03-46c6-bde8-d9f51be7f7da");
        }

        private void AddEventWithPrimtiveTypeToSelfDescription()
        {
            Event myEvent = new Event(
                "Id of my simple event",
                "Name of my simple event",
                "Description of my simple event",
                typeof(string));
            this.mySmartobject.AddEvent(myEvent);
        }

        private void AddEventWithComplexTypeToSelfDescription()
        {
            this.myEvent = new Event(
                "Id of my complex event",
                "Name of my complex event",
                "Description of my complex event",
                typeof(MyComplexEvent));
            this.mySmartobject.AddEvent(this.myEvent);
        }

        private void AddFunctionToSelfDescription()
        {
            MethodInfo methodInfo = this.GetType().GetRuntimeMethod("SampleMsbFunction", new Type[] { typeof(string), typeof(int), typeof(FunctionCallInfo) });
            Function sampleMsbFunction = new Function(methodInfo, this);
            this.mySmartobject.AddFunction(sampleMsbFunction);
        }

#pragma warning disable SA1202 // Elements must be ordered by access
        [MsbFunction(
            Id = "SampleFunction",
            Name = "Sample Function",
            Description = "Description of Sample Function")]
        public void SampleMsbFunction(
            [MsbFunctionParameter(Name = "NameOfStringParameterInDataFormat")] string stringParaneter,
            [MsbFunctionParameter(Name = "NameOfIntParameterInDataFormat")] int intParameter,
            FunctionCallInfo functionCallInfo)
        {
            Console.WriteLine($"Function Call via MSB: {stringParaneter} | {intParameter}");
        }
#pragma warning restore SA1202 // Elements must be ordered by access

        private void AddFunctionHandlerToSelfDescription()
        {
            this.mySmartobject.AddEvent(new Event("ResponseEvent1", "Response Event 1", "Description of Response Event 1", typeof(string)));
            this.mySmartobject.AddEvent(new Event("ResponseEvent2", "Response Event 2", "Description of Response Event 2", typeof(int)));
            SimpleFunctionHandler simpleFunctionHandler = new SimpleFunctionHandler();
            this.mySmartobject.AddFunctionHandler(simpleFunctionHandler);
        }

        private void AddConfigurationParametersToSelfDescription()
        {
            this.mySmartobject.AddConfigurationParameter("Parameter1", new ConfigurationParameterValue(123));
            this.mySmartobject.AddConfigurationParameter("Parameter2", new ConfigurationParameterValue("SampleValue"));
        }

        private void GetConfigurationParmaeters()
        {
            // Get configuration parameter using name of parameter as key
            var parameter1Value = this.mySmartobject.Configuration.Parameters["Parameter1"];
            Log.Information($"Value of configuration parameter: {parameter1Value}");
        }

        private void ConnectClient()
        {
            this.myMsbClient.ConnectAsync().Wait();
        }

        private void RegisterService()
        {
            this.myMsbClient.RegisterAsync(this.mySmartobject).Wait();
        }

        private void PublishEvent()
        {
            var eventData = new EventDataBuilder(this.myEvent)
                .SetCorrelationId("a5fc5da1-e7fa-4f63-bba9-63d07faa9783")
                .SetEventPriority(EventPriority.HIGH)
                .SetPublishingDate(DateTime.Now)
                .SetShouldBeCached(true)
                .SetValue(new MyComplexEvent())
                .Build();
            this.myMsbClient.PublishAsync(this.mySmartobject, eventData).Wait();
        }
    }

    #pragma warning disable SA1402 // File may only contain a single class
    public class MyComplexEvent
    {
        public string StringProperty { get; }

        public int IntProperty { get; }

        public string[] StringArrayProperty { get; }

        public List<float> FloatListProperty { get; }
    }
    #pragma warning restore SA1402 // File may only contain a single class

    [MsbFunctionHandler(Id = "SimpleFunctionHandlerId")]
    #pragma warning disable SA1402 // File may only contain a single class
    public class SimpleFunctionHandler : AbstractFunctionHandler
    {
        [MsbFunction(
            Id = "SampleFunction",
            Name = "Sample Function",
            Description = "Description of Sample Function")]
        public void SampleMsbFunction(
            [MsbFunctionParameter(Name = "NameOfStringParameterInDataFormat")] string stringParaneter,
            [MsbFunctionParameter(Name = "NameOfIntParameterInDataFormat")] int intParameter,
            FunctionCallInfo functionCallInfo)
        {
            Console.WriteLine($"Function Call via MSB: {stringParaneter} | {intParameter}");
        }

        [MsbFunction(
            Id = "AnotherFunction",
            Name = "Another Function",
            Description = "Description of Another Function",
            ResponseEvents = new string[] { "ResponseEvent1", "ResponseEvent2" })]
        public EventData SampleMsbFunction(
            FunctionCallInfo functionCallInfo)
        {
            Console.WriteLine($"Function Call via MSB");

            return new EventDataBuilder(functionCallInfo.ResponseEvents["ResponseEvent1"])
                .SetEventPriority(EventPriority.HIGH)
                .SetShouldBeCached(true)
                .SetValue(new MyComplexEvent())
                .Build();
        }
    }
    #pragma warning restore SA1402 // File may only contain a single class
}