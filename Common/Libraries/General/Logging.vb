Imports NLog.Config
Imports NLog.Targets
Imports Common.General.Variables
Imports Common.General.ProjectOperations

Namespace General
    Public Class Logging
        Public Shared Sub SetupLogger()
            ' Step 1. Create configuration object 
            Dim config = New LoggingConfiguration()

            ' Step 2. Create targets and add them to the configuration 
            'Dim consoleTarget = New ColoredConsoleTarget()
            'config.AddTarget("console", consoleTarget)

            Dim fileTarget = New FileTarget()
            config.AddTarget("file", fileTarget)

            ' Step 3. Set target properties 
            'consoleTarget.Layout = "${date:format=HH\:mm\:ss} ${logger} ${message}"
            fileTarget.FileName = "..\Logs\" & GetBuildLogFileName()
            fileTarget.Layout = "${message}"

            ' Step 4. Define rules
            'Dim rule1 = New LoggingRule("*", NLog.LogLevel.Debug, consoleTarget)
            'config.LoggingRules.Add(rule1)

            Dim rule2 = New LoggingRule("*", NLog.LogLevel.Debug, fileTarget)
            config.LoggingRules.Add(rule2)

            ' Step 5. Activate the configuration
            NLog.LogManager.Configuration = config

            ' Example usage
            logger = NLog.LogManager.GetLogger("WebRAD")
        End Sub

        Public Shared Function GetBuildLogFileName() As String
            Return currentProject.GetProjectNameAlphaNumericOnly() & "\" & Now().ToString().Replace("\", "_").Replace("/", "_").Replace(" ", "-") & ".txt"
        End Function
    End Class
End Namespace