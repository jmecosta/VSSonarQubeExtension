﻿namespace SonarRestService.Test

open NUnit.Framework
open SonarRestService
open Foq
open System.IO

open VSSonarPlugins
open VSSonarPlugins.Types
open System.Reflection

type AdministrationTests() =
   
    let assemblyRunningPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString()

    [<Test>]
    member test.``Should Get Users`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/users/search") @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/userList.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        let userList = (service :> ISonarRestService).GetUserList(conf)
        Assert.That(userList.Count, Is.EqualTo(3))

    [<Test>]
    member test.``Should Not Get Users for Server Less than 3.6`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/users/search") @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/NonExistentPage.xml"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        let userList = (service :> ISonarRestService).GetUserList(conf)
        Assert.That(userList.Count, Is.EqualTo(0))

    [<Test>]
    member test.``Should Authenticate User`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/authentication/validate") @>).Returns(""" {"valid":true} """)
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).AuthenticateUser(conf), Is.True)


    [<Test>]
    member test.``Should Not Authenticate User`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/authentication/validate") @>).Returns(""" {"valid":false} """)
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).AuthenticateUser(conf), Is.False)

    [<Test>]
    member test.``Should Fail authentication When Sonar less than 3.3 so skip authetication`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/authentication/validate") @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/NonExistentPage.xml"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).AuthenticateUser(conf), Is.False)

    [<Test>]
    member test.``Should Get Correct server version with 3.6`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3.6","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Should Get Correct server version with 3,6`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3,6","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Should Get Correct server version with 3.6-SNAPSHOT`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3.6-SNAPSHOT","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Should Get Correct server version with 3,6-SNAPSHOT`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3,6-SNAPSHOT","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Should Get Correct server version with 3.6.1`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3.6.1","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Should Get Correct server version with 3.6.1-SNAPSHOT`` () =
        let conf = ConnectionConfiguration("http://localhost:9000", "jocs1", "jocs1", 4.5)

        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/server") @>).Returns("""{"id":"20130712144608","version":"3.6.1-SNAPSHOT","status":"UP"}""")
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetServerInfo(conf), Is.EqualTo(3.6f))

    [<Test>]
    member test.``Get Properties`` () =
        let conf = ConnectionConfiguration("http://sonar", "jocs1", "jocs1", 4.5)
        let mockHttpReq =
            Mock<IHttpSonarConnector>()
                .Setup(fun x -> <@ x.HttpSonarGetRequest(any(), "/api/properties") @>).Returns(File.ReadAllText(assemblyRunningPath + "/testdata/PropertiesResponse.txt"))
                .Create()

        let service = SonarRestService(mockHttpReq)
        Assert.That((service :> ISonarRestService).GetProperties(conf).Count, Is.EqualTo(66))
