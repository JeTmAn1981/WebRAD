Imports Common.Actions

Namespace Actions
public Class PostbackActionData
        Public handlers As New StringBuilder()
        Public triggers As String

        Public Overrides Function ToString() As String
            Dim data as String

            data &= "postback handlers:<br /><br />" & handlers.ToString()
            data &= "postback Action Triggers:<br /><br />" & triggers
            
            return data
        End Function
End Class
    end namespace