﻿namespace SonarRestService.Test

open NUnit.Framework
open SonarRestService
open Foq
open System.IO

open VSSonarPlugins
open VSSonarPlugins.Types
open System.Reflection
type QualityTests() =
    let assemblyRunningPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString()
    [<Test>]
    member test.``Should Get A correct Quality Profile`` () =
        let conf = ConnectionConfiguration("host", "user", "password", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), any()) @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/profileresponse.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        let profile = (service :> ISonarRestService).GetEnabledRulesInProfile(conf , "language", "profile")
        Assert.That(profile.Count, Is.EqualTo(1))
        Assert.That(profile.[0].Alerts.Count, Is.EqualTo(6))
        Assert.That(profile.[0].GetAllRules().Count, Is.EqualTo(784))

    [<Test>]
    member test.``Should Get Valid Profile from Resource Response`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/resources?resource=project&metrics=profile") @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/qualityProfileOfResource.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        let resourceinfo = (service :> ISonarRestService).GetQualityProfile(conf, "project")
        Assert.That(resourceinfo.Count, Is.EqualTo(1))
        Assert.That(resourceinfo.[0].Name, Is.EqualTo("bla"))
        Assert.That(resourceinfo.[0].Metrics.[0].Key, Is.EqualTo("profile"))

    [<Test>]
    member test.``Should Get Rules From Profile using App`` () =
        let conf = ConnectionConfiguration("http://sonar", "user", "pass", 4.5)
       
        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), any()) @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/rulesearchquery.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)

        let profile = Profile(service, conf)
        profile.Key <- "msbuild-sonar-way-77787"
        profile.Language <- "msbuild"
        (service :> ISonarRestService).GetRulesForProfileUsingRulesApp(conf, profile, true)
        Assert.That(profile.GetAllRules().Count, Is.EqualTo(3))

    [<Test>]
    member test.``Should Get Rules From Profile`` () =
        let conf = ConnectionConfiguration("http://sonar", "user", "user", 4.5)
       
        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/profiles/index?language=cs&name=Sonar+way") @>)
                .Returns(File.ReadAllText(assemblyRunningPath + "/testdata/profileusingappid.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)

        let profile = Profile(service, conf)
        profile.Name <- "Sonar way"
        profile.Language <- "cs"
        (service :> ISonarRestService).GetRulesForProfile(conf, profile, false)
        Assert.That(profile.GetAllRules().Count, Is.EqualTo(199))

    [<Test>]
    member test.``Should Search Rule in Profile`` () =
        let conf = ConnectionConfiguration("http://sonar", "user", "pass", 4.5)
       
        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), any()) @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/rulesearchanswer.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)

        let profile = Profile(service, conf)
        profile.Key <- "msbuild-sonar-way-77787"
        profile.Language <- "msbuild"
        let rule = (service :> ISonarRestService).GetRuleUsingProfileAppId(conf, "csharpsquid:S108")
        Assert.That(rule.ConfigKey, Is.EqualTo("csharpsquid:S108"))

    [<Test>]
    member test.``Should GetActionPlans`` () =
        let conf = ConnectionConfiguration("http://sonar", "user", "user", 4.5)
       
        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), any()) @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/actionPlanData.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)

        let gates = (service :> ISonarRestService).GetAvailableActionPlan(conf, "tekla.structures.core:Common")
        Assert.That(gates.Count, Is.EqualTo(2))
        Assert.That(gates.[0].Name, Is.EqualTo("Version 3.6"))
        Assert.That(gates.[1].Name, Is.EqualTo("Version 3.5"))

