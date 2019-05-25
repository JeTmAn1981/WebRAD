Imports Common.Actions

Namespace Actions
public Class ActionData
        public postback As new PostbackActionData()
        public js As new JSActionData()

        Public Overrides Function ToString() As String
            Dim data as String

            'data = "Postback handlers:<br /><br />" & postbackactions
            'data &= "Postback Action Triggers:<br /><br />" & postbackactiontriggers
            'data &= "JS handlers:<br /><br />" & jsactions
            'data &= "JS Action Triggers:<br /><br />" & JSActionTriggers
            'data &= "JS handlers Handler Adds:<br /><br />" & jsactionhandleradds

            return data
        End Function
End Class
    end namespace