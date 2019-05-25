Imports Common.Actions

Namespace Actions
public Class JSActionData
        Public triggers, registrations As String
        Public handlers As New StringBuilder()

        Public Overrides Function ToString() As String
            Dim data as String

            data &= "JS handlers:<br /><br />" & handlers.ToString()
            data &= "JS Action Triggers:<br /><br />" & triggers
            data &= "JS handlers Handler Adds:<br /><br />" & registrations

            return data
        End Function
End Class
    end namespace