﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunAnalysisTests.fs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// --------------------------------------------------------------------------------------------------------------------
namespace SonarLocalAnalyser.Test

open NUnit.Framework
open FsUnit
open SonarLocalAnalyser
open Foq
open System.IO
open VSSonarPlugins

open SonarRestService
open VSSonarPlugins.Types

type RunAnalysisTests() =

    [<Test>]
    member test.``If Thread is Null then analysis is not running`` () =

        let mockConfReq =
            Mock<IConfigurationHelper>()
                .Setup(fun x -> <@ x.ReadSetting(any(), any(), any()) @>).Returns(new SonarQubeProperties(Value = "something"))
                .Create()

        let analyser = new SonarLocalAnalyser(null, Mock<ISonarRestService>().Create(), mockConfReq, Mock<ISonarConfiguration>().Create(), Mock<INotificationManager>().Create())
        ((analyser :> ISonarLocalAnalyser).IsExecuting()) |> should be False

    [<Test>]
    [<ExpectedException>]
    member test.``Should throw exception when no plugin is found`` () =
        let analyser = new SonarLocalAnalyser(null, Mock<ISonarRestService>().Create(), Mock<IConfigurationHelper>().Create(), Mock<ISonarConfiguration>().Create(), Mock<INotificationManager>().Create())
        ((analyser :> ISonarLocalAnalyser).GetResourceKey(new VsFileItem(), true)) |> should throw typeof<ResourceNotSupportedException>

    [<Test>]
    [<ExpectedException>]
    member test.``Should throw exception if No plugins are loaded and we give a good resource`` () =
        let analyser = new SonarLocalAnalyser(null, Mock<ISonarRestService>().Create(), Mock<IConfigurationHelper>().Create(), Mock<ISonarConfiguration>().Create(), Mock<INotificationManager>().Create())
        let project = new Resource()
        project.Lang <- "c++"
        ((analyser :> ISonarLocalAnalyser).GetResourceKey(new VsFileItem(), true)) |> should throw typeof<ResourceNotSupportedException>

    [<Test>]
    member test.``Should Return Key When Single language is set`` () =
        let project = new Resource()
        let vsItem = new VsFileItem(FileName = "fileName", FilePath = "filePath")
        let pluginDescription = new PluginDescription()
        pluginDescription.Name <- "TestPlugin"

        let mockAVsinterface =
            Mock<IConfigurationHelper>()
                .Setup(fun x -> <@ x.ReadSetting(any(), any(), any()) @>).Returns(new SonarQubeProperties(Value = "true"))
                .Create()

        let mockAPlugin =
            Mock<IAnalysisPlugin>()
                .Setup(fun x -> <@ x.IsSupported(vsItem) @>).Returns(true)
                .Setup(fun x -> <@ x.GetResourceKey(any(), any()) @>).Returns("Key")
                .Setup(fun x -> <@ x.GetPluginDescription() @>).Returns(pluginDescription)
                .Create()

        let listofPlugins = new System.Collections.Generic.List<IAnalysisPlugin>()
        listofPlugins.Add(mockAPlugin)
        let analyser = new SonarLocalAnalyser(listofPlugins, Mock<ISonarRestService>().Create(), mockAVsinterface, Mock<ISonarConfiguration>().Create(), Mock<INotificationManager>().Create())
        
        
        (analyser :> ISonarLocalAnalyser).GetResourceKey(vsItem, true) |> should equal "Key"