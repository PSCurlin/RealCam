'MICROCHIP SOFTWARE NOTICE AND DISCLAIMER:  You may use this software, and any derivatives created by any person or entity by or on your behalf, exclusively with Microchip’s products.  Microchip and its licensors retain all ownership and intellectual property rights in the accompanying software and in all derivatives hereto.  
'This software and any accompanying information is for suggestion only.  It does not modify Microchip’s standard warranty for its products.  You agree that you are solely responsible for testing the software and determining its suitability.  Microchip has no obligation to modify, test, certify, or support the software.
'THIS SOFTWARE IS SUPPLIED BY MICROCHIP "AS IS".  NO WARRANTIES, WHETHER EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE APPLY TO THIS SOFTWARE, ITS INTERACTION WITH MICROCHIP’S PRODUCTS, COMBINATION WITH ANY OTHER PRODUCTS, OR USE IN ANY APPLICATION. 
'IN NO EVENT, WILL MICROCHIP BE LIABLE, WHETHER IN CONTRACT, WARRANTY, TORT (INCLUDING NEGLIGENCE OR BREACH OF STATUTORY DUTY), STRICT LIABILITY, INDEMNITY, CONTRIBUTION, OR OTHERWISE, FOR ANY INDIRECT, SPECIAL, PUNITIVE, EXEMPLARY, INCIDENTAL OR CONSEQUENTIAL LOSS, DAMAGE, FOR COST OR EXPENSE OF ANY KIND WHATSOEVER RELATED TO THE SOFTWARE, HOWSOEVER CAUSED, EVEN IF MICROCHIP HAS BEEN ADVISED OF THE POSSIBILITY OR THE DAMAGES ARE FORESEEABLE.  TO THE FULLEST EXTENT ALLOWABLE BY LAW, MICROCHIP'S TOTAL LIABILITY ON ALL CLAIMS IN ANY WAY RELATED TO THIS SOFTWARE WILL NOT EXCEED THE AMOUNT OF FEES, IF ANY, THAT YOU HAVE PAID DIRECTLY TO MICROCHIP FOR THIS SOFTWARE.
'MICROCHIP PROVIDES THIS SOFTWARE CONDITIONALLY UPON YOUR ACCEPTANCE OF THESE TERMS.


Imports MCP2210
Imports MCP2210.DllConstants
Imports Microsoft.VisualBasic

'===========================================================
Public Class Form_Terminal

    Public Const VID = &H4D8
    Public Const PID = &HDE

    Public Const cNUM_GPs = 9

    Public uiPinDesignation As UInt16

    Public bPinDes As Boolean
    Public bIdleCSVal As Boolean
    Public bActiveCSVal As Boolean
    Public bSpiDataHexMode As Boolean

    Public ucPinDes(cNUM_GPs - 1), ucIdleCSVal(cNUM_GPs - 1), ucActiveCSVal(cNUM_GPs - 1) As Char

    ' SPI related
    Public TxData(65535 - 1), RxData(65535 - 1) As Byte
    Public paramBitRateValue As UInteger
    Public paramSpiMode As Byte
    Public paramNumBytes2Xfer As UInteger
    Public paramCS2DataDly As UInteger
    Public paramData2DataDly As UInteger
    Public paramData2CSDly As UInteger

    Public paramIdleCS As UInteger
    Public paramActiveCS As UInteger
    Public paramGPDesignation(cNUM_GPs - 1) As Byte

    ' MCP2210 DLL related
    Public UsbSpi As MCP2210.DevIO
    Public bMCP2210Connection As Boolean

    '===========================================================
    '===========================================================
    Private Sub Form_Terminal_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        bPinDes = False
        bIdleCSVal = False
        bActiveCSVal = False

        bSpiDataHexMode = False

        'now call the UpdateGPValues()
        UpdateGPValues()

        'now call the MCP2210 DLL functions
        UsbSpi = New MCP2210.DevIO(VID, PID)

        bMCP2210Connection = UsbSpi.Settings.GetConnectionStatus()

        If (bMCP2210Connection <> True) Then
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: MCP2210 not detected"
            ToolStripStatusLabel_AppStatus.Text = "MCP2210 Status: NOT CONNECTED"
            MessageBox.Show("MCP2210 not found!")
            'Me.Close()
        Else
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: MCP2210 detected"
            ToolStripStatusLabel_AppStatus.Text = "MCP2210 Status: CONNECTED"
        End If

        'set the number of visible rows to 1
        DataGridView_TxData.RowCount = 1
        DataGridView_RxData.RowCount = 1

        'now enable the timer so we will update the APP status with the MCP2210 connection state
        Timer1.Enabled = True

    End Sub
    '===========================================================
    '===========================================================
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'use the timer callback to update the state of the MCP2210 connection
        bMCP2210Connection = UsbSpi.Settings.GetConnectionStatus()

        If (bMCP2210Connection <> True) Then
            ToolStripStatusLabel_AppStatus.Text = "MCP2210 Status: NOT CONNECTED"
        Else
            ToolStripStatusLabel_AppStatus.Text = "MCP2210 Status: CONNECTED"
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub Button_TxferSPIData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_TxferSPIData.Click
        'prepare the parameters
        Dim bResult As Boolean
        Dim iResult As Integer

        bResult = GetSpiParameters()

        If (bResult = False) Then
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Error getting the SPI parameters"
            Return
        End If

        'temporary stop the timer
        Timer1.Stop()

        'use the timer callback to update the state of the MCP2210 connection
        bMCP2210Connection = UsbSpi.Settings.GetConnectionStatus()

        If (bMCP2210Connection <> True) Then
            ToolStripStatusLabel_AppStatus.Text = "MCP2210 Status: NOT CONNECTED"
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Error getting communicating with MCP2210"
            'resume the timer counting
            Timer1.Start()
            Return
        End If

        'set the GP designations
        iResult = UsbSpi.Settings.SetGpioConfig(CURRENT_SETTINGS_ONLY,
                                                paramGPDesignation, &HFFFF, &HFFFF)

        If (iResult <> 0) Then
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: TxFerSpiData error setting GPs"
            'resume the timer counting
            Timer1.Start()
            Return
        End If

        'now that we have the SPI parameters, we will setup the chip
        iResult = UsbSpi.Settings.SetAllSpiSettings(CURRENT_SETTINGS_ONLY,
                                                    paramBitRateValue,
                                                    paramIdleCS,
                                                    paramActiveCS,
                                                    paramCS2DataDly,
                                                    paramData2DataDly,
                                                    paramData2CSDly,
                                                    paramNumBytes2Xfer,
                                                    paramSpiMode)

        If (iResult <> 0) Then
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: TxFerSpiData error setting SPI parameters"
            'resume the timer counting
            Timer1.Start()
            Return
        End If

        'now call the SPI transfer function
        iResult = UsbSpi.Functions.TxferSpiData(TxData, RxData)

        If (iResult <> 0) Then
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: TxFerSpiData error transferring SPI data"
            'resume the timer counting
            Timer1.Start()
            Return
        End If

        'get the current chip settings and update the form
        paramBitRateValue = UsbSpi.Settings.GetSpiBitRate(CURRENT_SETTINGS_ONLY)
        paramSpiMode = UsbSpi.Settings.GetSpiMode(CURRENT_SETTINGS_ONLY)
        paramNumBytes2Xfer = UsbSpi.Settings.GetSpiTxferSize(CURRENT_SETTINGS_ONLY)
        paramCS2DataDly = UsbSpi.Settings.GetSpiDelayCsToData(CURRENT_SETTINGS_ONLY)
        paramData2DataDly = UsbSpi.Settings.GetSpiDelayDataToData(CURRENT_SETTINGS_ONLY)
        paramData2CSDly = UsbSpi.Settings.GetSpiDelayDataToCs(CURRENT_SETTINGS_ONLY)
        UpdateSPIParameters()

        'update the RxData data grid view cells
        Dim data_cell As DataGridViewCell
        Dim data_row As DataGridViewRow
        Dim counter As Integer

        If (paramNumBytes2Xfer > 0) Then
            DataGridView_RxData.RowCount = paramNumBytes2Xfer
        Else
            'when no data needs to be transferred, keep at least 1 row
            DataGridView_RxData.RowCount = 1
        End If

        If (bSpiDataHexMode = True) Then
            'update the RX Data grid with HEX values
            For counter = 0 To (DataGridView_RxData.Rows.Count - 1) Step 1
                'and for RX Data as well
                data_row = DataGridView_RxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                data_cell.Value = RxData(counter).ToString("X2")
            Next
        Else
            'update the RX Data grid with DEC values
            For counter = 0 To (DataGridView_RxData.Rows.Count - 1) Step 1
                'and for RX Data as well
                data_row = DataGridView_RxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                data_cell.Value = RxData(counter).ToString("d3")
            Next
        End If

        'update the data grid format
        If (CheckBox_HexMode.Checked = True) Then
            UpdateSPIDataFormat(True)
        Else
            UpdateSPIDataFormat(False)
        End If

        'resume the timer counting
        Timer1.Start()

    End Sub
    '===========================================================
    '===========================================================
    '
    '
    '
    '
    '   From this line below, the code is used only to 
    '   validate/display the SPI data and its parameters
    '   
    '   Assuming the user's application has its own parameter
    '   validation, one can use the code till this line in order
    '   to know how to use the provided managed MCP2210 DLL
    '
    '
    '
    '
    '
    '===========================================================
    '===========================================================
    Private Sub DataGridView_TxData_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView_TxData.CellEndEdit
        Dim data_cell As DataGridViewCell
        Dim data_row As DataGridViewRow
        Dim strCellVal As String
        Dim iCellVal As Integer

        data_row = DataGridView_TxData.Rows.Item(e.RowIndex)
        data_cell = data_row.Cells.Item(e.ColumnIndex)

        strCellVal = data_cell.Value

        Try
            If (CheckBox_HexMode.Checked = True) Then
                iCellVal = System.Convert.ToByte(strCellVal, 16)
            Else
                iCellVal = System.Convert.ToByte(strCellVal, 10)
            End If
        Catch ex As Exception
            'keep the old value - do nothing
            iCellVal = 0
        End Try

        TxData(e.RowIndex) = iCellVal
        If (CheckBox_HexMode.Checked = True) Then
            data_cell.Value = iCellVal.ToString("X2")
        Else
            data_cell.Value = iCellVal.ToString("d3")
        End If

    End Sub
    '===========================================================
    Public Sub UpdateSPIParameters()
        'update the SPI textboxes
        TextBox_BitRate.Text = paramBitRateValue
        TextBox_SpiMode.Text = paramSpiMode
        TextBox_NumBytes2Xfer.Text = paramNumBytes2Xfer
        TextBox_CS2DataDly.Text = paramCS2DataDly
        TextBox_Data2DataDly.Text = paramData2DataDly
        TextBox_Data2CSDly.Text = paramData2CSDly
    End Sub
    '===========================================================
    Public Sub UpdateSPIDataFormat(ByVal hexmode As Boolean)
        Dim data_cell As DataGridViewCell
        Dim data_row As DataGridViewRow
        Dim strCellVal As String
        Dim ucCellVal As Byte
        Dim counter As Integer

        If ((hexmode <> bSpiDataHexMode) And
            (hexmode = True)) Then
            bSpiDataHexMode = True
            'convert the data to hex
            'the upper limit is the number of rows - 2, because the last row is empty
            For counter = 0 To (DataGridView_TxData.Rows.Count - 2) Step 1
                'do the conversion for TX Data
                data_row = DataGridView_TxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                strCellVal = data_cell.Value
                ucCellVal = System.Convert.ToByte(strCellVal, 10)
                'data_cell.Value = ucCellVal.ToString("X2")
                data_cell.Value = TxData(counter).ToString("X2")
            Next
            'the upper limit is the number of rows - 2, because the last row is empty
            For counter = 0 To (DataGridView_RxData.Rows.Count - 2) Step 1
                'and for RX Data as well
                data_row = DataGridView_RxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                strCellVal = data_cell.Value
                ucCellVal = System.Convert.ToByte(strCellVal, 10)
                'data_cell.Value = ucCellVal.ToString("X2")
                data_cell.Value = RxData(counter).ToString("X2")
            Next
        End If
        If ((hexmode <> bSpiDataHexMode) And
            (hexmode = False)) Then
            bSpiDataHexMode = False
            'convert the data from hex
            'the upper limit is the number of rows - 2, because the last row is empty
            For counter = 0 To (DataGridView_TxData.Rows.Count - 2) Step 1
                'do the conversion for TX Data
                data_row = DataGridView_TxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                strCellVal = data_cell.Value
                ucCellVal = System.Convert.ToByte(strCellVal, 16)
                'data_cell.Value = ucCellVal.ToString("d3")
                data_cell.Value = TxData(counter).ToString("d3")
            Next
            'the upper limit is the number of rows - 2, because the last row is empty
            For counter = 0 To (DataGridView_RxData.Rows.Count - 2) Step 1
                'and for RX Data as well
                data_row = DataGridView_RxData.Rows.Item(counter)
                data_cell = data_row.Cells.Item(0)
                strCellVal = data_cell.Value
                ucCellVal = System.Convert.ToByte(strCellVal, 16)
                'data_cell.Value = ucCellVal.ToString("d3")
                data_cell.Value = RxData(counter).ToString("d3")
            Next
        End If
    End Sub
    '===========================================================
    '===========================================================
    Public Function GetSpiParameters() As Boolean
        'convert the bitrate
        Try
            paramBitRateValue = System.Convert.ToUInt32(TextBox_BitRate.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: BitRate value wrong"
            Return False
        End Try

        'convert the SpiMode
        Try
            paramSpiMode = System.Convert.ToUInt32(TextBox_SpiMode.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Spi Mode value wrong"
            Return False
        End Try

        'convert the NumBytes2Xfer
        Try
            paramNumBytes2Xfer = System.Convert.ToUInt32(TextBox_NumBytes2Xfer.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Number of bytes for transfer value wrong"
            Return False
        End Try

        'convert the CS2DataDly
        Try
            paramCS2DataDly = System.Convert.ToUInt32(TextBox_CS2DataDly.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: CS to Data delay value wrong"
            Return False
        End Try

        'convert the Data2DataDly
        Try
            paramData2DataDly = System.Convert.ToUInt32(TextBox_Data2DataDly.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Data to Data delay value wrong"
            Return False
        End Try

        'convert the Data2CSDly
        Try
            paramData2CSDly = System.Convert.ToUInt32(TextBox_Data2CSDly.Text, 10)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: Data to CS delay value wrong"
            Return False
        End Try

        Dim strValue As String

        'convert the IDLE CS value
        strValue = CStr(ucIdleCSVal)
        Try
            paramIdleCS = System.Convert.ToUInt16(strValue, 2)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: IDLE CS delay value wrong"
            Return False
        End Try

        'convert the ACTIVE CS value
        strValue = CStr(ucActiveCSVal)
        Try
            paramActiveCS = System.Convert.ToUInt16(strValue, 2)
        Catch ex As Exception
            'abort the operation
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: ACTIVE CS delay value wrong"
            Return False
        End Try

        Dim counter As Integer
        'convert the GP designation
        For counter = 0 To TextBox_PinDes.TextLength - 1 Step 1
            If (TextBox_PinDes.Text(counter) = "0"c) Then
                'reverse the order because the SPI function takes this parameter with the value of index 0
                'corresponding to the GP0 designation
                'TextBox_PinDes.Text has the GP8 designation on index 0
                paramGPDesignation(cNUM_GPs - counter - 1) = 0
            End If

            If (TextBox_PinDes.Text(counter) = "1"c) Then
                paramGPDesignation(cNUM_GPs - counter - 1) = 1
            End If

            If (TextBox_PinDes.Text(counter) = "2"c) Then
                paramGPDesignation(cNUM_GPs - counter - 1) = 2
            End If
        Next

        'check if we need to fill the SPI TX user data
        Dim ucFillValue As Byte

        If (CheckBox_HexMode.Checked = True) Then
            ucFillValue = System.Convert.ToByte(TextBox_FillData.Text, 16)
        Else
            ucFillValue = System.Convert.ToByte(TextBox_FillData.Text, 10)
        End If

        If (CheckBox_FillData.Checked = True) Then
            For counter = (DataGridView_TxData.Rows.Count - 1) To (paramNumBytes2Xfer - 1) Step 1
                TxData(counter) = ucFillValue
            Next
        End If

        'everything went well
        Return True
    End Function
    '===========================================================
    '===========================================================
    '===========================================================
    
    '===========================================================
    '===========================================================
    Private Sub CheckBox_HexMode_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_HexMode.CheckedChanged
        If CheckBox_HexMode.Checked = True Then
            CheckBox_FillData.Text = "Fill remaining with (HEX):"
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: HEX mode active"
            'update the Tx/Rx SPI Data values
            UpdateSPIDataFormat(True)
        Else
            CheckBox_FillData.Text = "Fill remaining with (DEC):"
            ToolStripStatusLabel_CmdStatus.Text = "Command Status: DEC mode active"
            'update the Tx/Rx SPI Data values
            UpdateSPIDataFormat(False)
        End If

        'Fill data is 0 when changing the modes
        TextBox_FillData.Text = "0"

    End Sub
    '===========================================================
    Private Sub TextBox_PinDes_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_PinDes.TextChanged
        Dim counter As Int16
        Dim strPinDes As String
        Dim strLength As Integer

        If (TextBox_PinDes.TextLength >= cNUM_GPs) Then
            strLength = cNUM_GPs
            'mark the length of the field is correct
            bPinDes = True
        Else
            strLength = TextBox_PinDes.TextLength
            'length is less than needed
            bPinDes = False
        End If

        strPinDes = TextBox_PinDes.Text.Substring(0, strLength)

        For counter = 0 To (strPinDes.Length - 1) Step 1
            If ((strPinDes.Chars(counter) = "0") Or
                (strPinDes.Chars(counter) = "1") Or
                (strPinDes.Chars(counter) = "2")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: GP Designation correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: GP Designation wrong value"
                strPinDes = strPinDes.Insert(counter, "x")
                strPinDes = strPinDes.Remove(counter + 1, 1)
                'if we got here at least one designator is out of valid range - invalidate the flag
                bPinDes = False
            End If
        Next

        TextBox_PinDes.Text = System.String.Copy(strPinDes)

        'now check if all the 3 fields are ok and if so update the other GP controls
        If ((bPinDes = True) And (bIdleCSVal = True) And (bActiveCSVal = True)) Then
            'call the update function for the GP controls
            UpdateGPControls()
        End If

    End Sub
    '===========================================================
    Private Sub TextBox_IdleCSValue_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_IdleCSValue.TextChanged
        Dim counter As Int16
        Dim strIdleCSValue As String
        Dim strLength As Integer

        If (TextBox_IdleCSValue.TextLength >= cNUM_GPs) Then
            strLength = cNUM_GPs
            'mark the length of the field is correct
            bIdleCSVal = True
        Else
            strLength = TextBox_IdleCSValue.TextLength
            'length is less than needed
            bIdleCSVal = False
        End If

        strIdleCSValue = TextBox_IdleCSValue.Text.Substring(0, strLength)

        For counter = 0 To (strIdleCSValue.Length - 1) Step 1
            If ((strIdleCSValue.Chars(counter) = "0") Or
                (strIdleCSValue.Chars(counter) = "1")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: IDLE CS correct bit value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: IDLE CS wrong bit value"
                strIdleCSValue = strIdleCSValue.Insert(counter, "x")
                strIdleCSValue = strIdleCSValue.Remove(counter + 1, 1)
                'if we got here at least one designator is out of valid range - invalidate the flag
                bIdleCSVal = False
            End If
        Next

        TextBox_IdleCSValue.Text = System.String.Copy(strIdleCSValue)

        'now check if all the 3 fields are ok and if so update the other GP controls
        If ((bPinDes = True) And (bIdleCSVal = True) And (bActiveCSVal = True)) Then
            'call the update function for the GP controls
            UpdateGPControls()
        End If
    End Sub
    '===========================================================
    Private Sub TextBox_ActiveCSValue_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_ActiveCSValue.TextChanged
        Dim counter As Int16
        Dim strActiveCSValue As String
        Dim strLength As Integer

        If (TextBox_ActiveCSValue.TextLength >= cNUM_GPs) Then
            strLength = cNUM_GPs
            'mark the length of the field is correct
            bActiveCSVal = True
        Else
            strLength = TextBox_ActiveCSValue.TextLength
            'length is less than needed
            bActiveCSVal = False
        End If

        strActiveCSValue = TextBox_ActiveCSValue.Text.Substring(0, strLength)

        For counter = 0 To (strActiveCSValue.Length - 1) Step 1
            If ((strActiveCSValue.Chars(counter) = "0") Or
                (strActiveCSValue.Chars(counter) = "1")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: ACTIVE CS correct bit value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: ACTIVE CS wrong bit value"
                strActiveCSValue = strActiveCSValue.Insert(counter, "x")
                strActiveCSValue = strActiveCSValue.Remove(counter + 1, 1)
                'if we got here at least one designator is out of valid range - invalidate the flag
                bActiveCSVal = False
            End If
        Next

        TextBox_ActiveCSValue.Text = System.String.Copy(strActiveCSValue)

        'now check if all the 3 fields are ok and if so update the other GP controls
        If ((bPinDes = True) And (bIdleCSVal = True) And (bActiveCSVal = True)) Then
            'call the update function for the GP controls
            UpdateGPControls()
        End If
    End Sub
    '===========================================================
    Private Sub TextBox_FillData_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_FillData.TextChanged
        Dim counter As Int16
        Dim strFillData As String
        Dim strLength As Integer
        Dim bGoodNumber As Boolean

        If (CheckBox_HexMode.Checked = False) Then
            If (TextBox_FillData.TextLength > 0) Then
                'a maximum of 3 digits are allowed for a decimal byte value
                If (TextBox_FillData.TextLength >= 3) Then
                    strLength = 3
                Else
                    strLength = TextBox_FillData.TextLength()
                End If

                bGoodNumber = True
                strFillData = TextBox_FillData.Text.Substring(0, strLength)

                For counter = 0 To (strFillData.Length - 1) Step 1
                    'only the below characters are allowed
                    If ((strFillData.Chars(counter) = "0") Or
                        (strFillData.Chars(counter) = "1") Or
                        (strFillData.Chars(counter) = "2") Or
                        (strFillData.Chars(counter) = "3") Or
                        (strFillData.Chars(counter) = "4") Or
                        (strFillData.Chars(counter) = "5") Or
                        (strFillData.Chars(counter) = "6") Or
                        (strFillData.Chars(counter) = "7") Or
                        (strFillData.Chars(counter) = "8") Or
                        (strFillData.Chars(counter) = "9")) Then
                        ToolStripStatusLabel_CmdStatus.Text = "Command Status: Fill Data(DEC) correct value"
                    Else
                        ToolStripStatusLabel_CmdStatus.Text = "Command Status: Fill Data(DEC) wrong value"
                        strFillData = strFillData.Insert(counter, "x")
                        strFillData = strFillData.Remove(counter + 1, 1)
                        bGoodNumber = False
                    End If
                Next

                If (bGoodNumber = True) Then
                    If (System.Convert.ToUInt16(strFillData) > 255) Then
                        strFillData = 255
                    End If
                End If

                TextBox_FillData.Text = System.String.Copy(strFillData)
            End If
        Else
            If (TextBox_FillData.TextLength > 0) Then
                'a maximum of 2 digits are allowed for a HEX byte value
                If (TextBox_FillData.TextLength >= 2) Then
                    strLength = 2
                Else
                    strLength = TextBox_FillData.TextLength()
                End If

                strFillData = TextBox_FillData.Text.Substring(0, strLength)
                strFillData = strFillData.ToUpper()

                For counter = 0 To (strFillData.Length - 1) Step 1
                    'only the below characters are allowed
                    If ((strFillData.Chars(counter) = "0") Or
                        (strFillData.Chars(counter) = "1") Or
                        (strFillData.Chars(counter) = "2") Or
                        (strFillData.Chars(counter) = "3") Or
                        (strFillData.Chars(counter) = "4") Or
                        (strFillData.Chars(counter) = "5") Or
                        (strFillData.Chars(counter) = "6") Or
                        (strFillData.Chars(counter) = "7") Or
                        (strFillData.Chars(counter) = "8") Or
                        (strFillData.Chars(counter) = "9") Or
                        (strFillData.Chars(counter) = "A") Or
                        (strFillData.Chars(counter) = "B") Or
                        (strFillData.Chars(counter) = "C") Or
                        (strFillData.Chars(counter) = "D") Or
                        (strFillData.Chars(counter) = "E") Or
                        (strFillData.Chars(counter) = "F")) Then
                        ToolStripStatusLabel_CmdStatus.Text = "Command Status: Fill Data(HEX) correct value"
                    Else
                        ToolStripStatusLabel_CmdStatus.Text = "Command Status: Fill Data(HEX) wrong value"
                        strFillData = strFillData.Insert(counter, "x")
                        strFillData = strFillData.Remove(counter + 1, 1)
                    End If
                Next

                TextBox_FillData.Text = System.String.Copy(strFillData)
            End If
        End If
    End Sub
    '===========================================================
    Private Sub TextBox_BitRate_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_BitRate.TextChanged
        Dim counter As Int16
        Dim strBitRateData As String
        Dim strLength As Integer

        'a maximum of 8 digits are allowed for the bitrate value
        If (TextBox_BitRate.TextLength >= 8) Then
            strLength = 8
        Else
            strLength = TextBox_BitRate.TextLength()
        End If

        strBitRateData = TextBox_BitRate.Text.Substring(0, strLength)

        For counter = 0 To (strBitRateData.Length - 1) Step 1
            'only the below characters are allowed
            If ((strBitRateData.Chars(counter) = "0") Or
                (strBitRateData.Chars(counter) = "1") Or
                (strBitRateData.Chars(counter) = "2") Or
                (strBitRateData.Chars(counter) = "3") Or
                (strBitRateData.Chars(counter) = "4") Or
                (strBitRateData.Chars(counter) = "5") Or
                (strBitRateData.Chars(counter) = "6") Or
                (strBitRateData.Chars(counter) = "7") Or
                (strBitRateData.Chars(counter) = "8") Or
                (strBitRateData.Chars(counter) = "9")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Bitrate correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Bitrate wrong value"
                strBitRateData = strBitRateData.Insert(counter, "x")
                strBitRateData = strBitRateData.Remove(counter + 1, 1)
            End If
        Next

        TextBox_BitRate.Text = System.String.Copy(strBitRateData)

    End Sub
    '===========================================================
    Private Sub TextBox_SpiMode_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_SpiMode.TextChanged
        Dim counter As Int16
        Dim strSpiMode As String
        Dim strLength As Integer

        'a maximum of 1 digit is allowed for the SPI mode
        If (TextBox_SpiMode.TextLength >= 1) Then
            strLength = 1
        Else
            strLength = TextBox_SpiMode.TextLength()
        End If

        strSpiMode = TextBox_SpiMode.Text.Substring(0, strLength)

        For counter = 0 To (strSpiMode.Length - 1) Step 1
            'only the below characters are allowed
            If ((strSpiMode.Chars(counter) = "0") Or
                (strSpiMode.Chars(counter) = "1") Or
                (strSpiMode.Chars(counter) = "2") Or
                (strSpiMode.Chars(counter) = "3")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: SPI Mode correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: SPI Mode wrong value"
                strSpiMode = strSpiMode.Insert(counter, "x")
                strSpiMode = strSpiMode.Remove(counter + 1, 1)
            End If
        Next

        TextBox_SpiMode.Text = System.String.Copy(strSpiMode)
    End Sub
    '===========================================================
    Private Sub TextBox_NumBytes2Xfer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_NumBytes2Xfer.TextChanged
        Dim counter As Int16
        Dim strNumBytes2Xfer As String
        Dim strLength As Integer
        Dim bGoodNumber As Boolean

        'a maximum of 5 digits are allowed for the number of bytes to be transferred
        If (TextBox_NumBytes2Xfer.TextLength >= 5) Then
            strLength = 5
        Else
            strLength = TextBox_NumBytes2Xfer.TextLength()
        End If

        bGoodNumber = True
        strNumBytes2Xfer = TextBox_NumBytes2Xfer.Text.Substring(0, strLength)

        For counter = 0 To (strNumBytes2Xfer.Length - 1) Step 1
            'only the below characters are allowed
            If ((strNumBytes2Xfer.Chars(counter) = "0") Or
                (strNumBytes2Xfer.Chars(counter) = "1") Or
                (strNumBytes2Xfer.Chars(counter) = "2") Or
                (strNumBytes2Xfer.Chars(counter) = "3") Or
                (strNumBytes2Xfer.Chars(counter) = "4") Or
                (strNumBytes2Xfer.Chars(counter) = "5") Or
                (strNumBytes2Xfer.Chars(counter) = "6") Or
                (strNumBytes2Xfer.Chars(counter) = "7") Or
                (strNumBytes2Xfer.Chars(counter) = "8") Or
                (strNumBytes2Xfer.Chars(counter) = "9")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Number of bytes to xfer correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Number of bytes to xfer wrong value"
                strNumBytes2Xfer = strNumBytes2Xfer.Insert(counter, "x")
                strNumBytes2Xfer = strNumBytes2Xfer.Remove(counter + 1, 1)
                bGoodNumber = False
            End If
        Next

        If (bGoodNumber = True) Then
            If (System.Convert.ToUInt32(strNumBytes2Xfer) > 65535) Then
                strNumBytes2Xfer = 65535
            End If

            If (System.Convert.ToUInt32(strNumBytes2Xfer) > 0) Then
                'update the SPI TX Data grid view number of rows
                DataGridView_TxData.RowCount = System.Convert.ToUInt32(strNumBytes2Xfer)
            Else
                'when there's 0 bytes to be displayed keep at least 1 row to avoid an exception
                DataGridView_TxData.RowCount = 1
            End If
        End If

        TextBox_NumBytes2Xfer.Text = System.String.Copy(strNumBytes2Xfer)
    End Sub
    '===========================================================
    Private Sub TextBox_CS2DataDly_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_CS2DataDly.TextChanged
        Dim counter As Int16
        Dim strCS2DataDly As String
        Dim strLength As Integer

        'a maximum of 5 digits are allowed for the number of bytes to be transferred
        If (TextBox_CS2DataDly.TextLength >= 5) Then
            strLength = 5
        Else
            strLength = TextBox_CS2DataDly.TextLength()
        End If

        strCS2DataDly = TextBox_CS2DataDly.Text.Substring(0, strLength)

        For counter = 0 To (strCS2DataDly.Length - 1) Step 1
            'only the below characters are allowed
            If ((strCS2DataDly.Chars(counter) = "0") Or
                (strCS2DataDly.Chars(counter) = "1") Or
                (strCS2DataDly.Chars(counter) = "2") Or
                (strCS2DataDly.Chars(counter) = "3") Or
                (strCS2DataDly.Chars(counter) = "4") Or
                (strCS2DataDly.Chars(counter) = "5") Or
                (strCS2DataDly.Chars(counter) = "6") Or
                (strCS2DataDly.Chars(counter) = "7") Or
                (strCS2DataDly.Chars(counter) = "8") Or
                (strCS2DataDly.Chars(counter) = "9")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: CS2Data delay correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: CS2Data delay wrong value"
                strCS2DataDly = strCS2DataDly.Insert(counter, "x")
                strCS2DataDly = strCS2DataDly.Remove(counter + 1, 1)
            End If
        Next

        TextBox_CS2DataDly.Text = System.String.Copy(strCS2DataDly)
    End Sub
    '===========================================================
    Private Sub TextBox_Data2DataDly_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_Data2DataDly.TextChanged
        Dim counter As Int16
        Dim strData2DataDly As String
        Dim strLength As Integer

        'a maximum of 5 digits are allowed for the number of bytes to be transferred
        If (TextBox_Data2DataDly.TextLength >= 5) Then
            strLength = 5
        Else
            strLength = TextBox_Data2DataDly.TextLength()
        End If

        strData2DataDly = TextBox_Data2DataDly.Text.Substring(0, strLength)

        For counter = 0 To (strData2DataDly.Length - 1) Step 1
            'only the below characters are allowed
            If ((strData2DataDly.Chars(counter) = "0") Or
                (strData2DataDly.Chars(counter) = "1") Or
                (strData2DataDly.Chars(counter) = "2") Or
                (strData2DataDly.Chars(counter) = "3") Or
                (strData2DataDly.Chars(counter) = "4") Or
                (strData2DataDly.Chars(counter) = "5") Or
                (strData2DataDly.Chars(counter) = "6") Or
                (strData2DataDly.Chars(counter) = "7") Or
                (strData2DataDly.Chars(counter) = "8") Or
                (strData2DataDly.Chars(counter) = "9")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Data2Data delay correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: Data2Data delay wrong value"
                strData2DataDly = strData2DataDly.Insert(counter, "x")
                strData2DataDly = strData2DataDly.Remove(counter + 1, 1)
            End If
        Next

        TextBox_Data2DataDly.Text = System.String.Copy(strData2DataDly)
    End Sub
    '===========================================================
    Private Sub TextBox_Data2CSDly_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox_Data2CSDly.TextChanged
        Dim counter As Int16
        Dim strData2CSDly As String
        Dim strLength As Integer

        'a maximum of 5 digits are allowed for the number of bytes to be transferred
        If (TextBox_Data2CSDly.TextLength >= 5) Then
            strLength = 5
        Else
            strLength = TextBox_Data2CSDly.TextLength()
        End If

        strData2CSDly = TextBox_Data2CSDly.Text.Substring(0, strLength)

        For counter = 0 To (strData2CSDly.Length - 1) Step 1
            'only the below characters are allowed
            If ((strData2CSDly.Chars(counter) = "0") Or
                (strData2CSDly.Chars(counter) = "1") Or
                (strData2CSDly.Chars(counter) = "2") Or
                (strData2CSDly.Chars(counter) = "3") Or
                (strData2CSDly.Chars(counter) = "4") Or
                (strData2CSDly.Chars(counter) = "5") Or
                (strData2CSDly.Chars(counter) = "6") Or
                (strData2CSDly.Chars(counter) = "7") Or
                (strData2CSDly.Chars(counter) = "8") Or
                (strData2CSDly.Chars(counter) = "9")) Then
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: DataCS delay correct value"
            Else
                ToolStripStatusLabel_CmdStatus.Text = "Command Status: DataCS delay wrong value"
                strData2CSDly = strData2CSDly.Insert(counter, "x")
                strData2CSDly = strData2CSDly.Remove(counter + 1, 1)
            End If
        Next

        TextBox_Data2CSDly.Text = System.String.Copy(strData2CSDly)
    End Sub
    '===========================================================


    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP0_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP0_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP0_CS_Idle.Checked = False
        CheckBox_GP0_CS_Idle.Enabled = False
        CheckBox_GP0_CS_Active.Checked = False
        CheckBox_GP0_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP0_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP0_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP0_CS_Idle.Checked = False
        CheckBox_GP0_CS_Idle.Enabled = False
        CheckBox_GP0_CS_Active.Checked = False
        CheckBox_GP0_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP0_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP0_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP0_CS_Idle.Enabled = True
        CheckBox_GP0_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP0_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP0_CS_Idle.Click
        If (CheckBox_GP0_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP0_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP0_CS_Active.Click
        If (CheckBox_GP0_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP1_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP1_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP1_CS_Idle.Checked = False
        CheckBox_GP1_CS_Idle.Enabled = False
        CheckBox_GP1_CS_Active.Checked = False
        CheckBox_GP1_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP1_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP1_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP1_CS_Idle.Checked = False
        CheckBox_GP1_CS_Idle.Enabled = False
        CheckBox_GP1_CS_Active.Checked = False
        CheckBox_GP1_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP1_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP1_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP1_CS_Idle.Enabled = True
        CheckBox_GP1_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP1_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP1_CS_Idle.Click
        If (CheckBox_GP1_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP1_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP1_CS_Active.Click
        If (CheckBox_GP1_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP2_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP2_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP2_CS_Idle.Checked = False
        CheckBox_GP2_CS_Idle.Enabled = False
        CheckBox_GP2_CS_Active.Checked = False
        CheckBox_GP2_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP2_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP2_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP2_CS_Idle.Checked = False
        CheckBox_GP2_CS_Idle.Enabled = False
        CheckBox_GP2_CS_Active.Checked = False
        CheckBox_GP2_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP2_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP2_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP2_CS_Idle.Enabled = True
        CheckBox_GP2_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP2_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP2_CS_Idle.Click
        If (CheckBox_GP2_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP2_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP2_CS_Active.Click
        If (CheckBox_GP2_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP3_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP3_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP3_CS_Idle.Checked = False
        CheckBox_GP3_CS_Idle.Enabled = False
        CheckBox_GP3_CS_Active.Checked = False
        CheckBox_GP3_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP3_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP3_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP3_CS_Idle.Checked = False
        CheckBox_GP3_CS_Idle.Enabled = False
        CheckBox_GP3_CS_Active.Checked = False
        CheckBox_GP3_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP3_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP3_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP3_CS_Idle.Enabled = True
        CheckBox_GP3_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP3_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP3_CS_Idle.Click
        If (CheckBox_GP3_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP3_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP3_CS_Active.Click
        If (CheckBox_GP3_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP4_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP4_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP4_CS_Idle.Checked = False
        CheckBox_GP4_CS_Idle.Enabled = False
        CheckBox_GP4_CS_Active.Checked = False
        CheckBox_GP4_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP4_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP4_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP4_CS_Idle.Checked = False
        CheckBox_GP4_CS_Idle.Enabled = False
        CheckBox_GP4_CS_Active.Checked = False
        CheckBox_GP4_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP4_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP4_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP4_CS_Idle.Enabled = True
        CheckBox_GP4_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP4_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP4_CS_Idle.Click
        If (CheckBox_GP4_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP4_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP4_CS_Active.Click
        If (CheckBox_GP4_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP5_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP5_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP5_CS_Idle.Checked = False
        CheckBox_GP5_CS_Idle.Enabled = False
        CheckBox_GP5_CS_Active.Checked = False
        CheckBox_GP5_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP5_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP5_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP5_CS_Idle.Checked = False
        CheckBox_GP5_CS_Idle.Enabled = False
        CheckBox_GP5_CS_Active.Checked = False
        CheckBox_GP5_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP5_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP5_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP5_CS_Idle.Enabled = True
        CheckBox_GP5_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP5_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP5_CS_Idle.Click
        If (CheckBox_GP5_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP5_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP5_CS_Active.Click
        If (CheckBox_GP5_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP6_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP6_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP6_CS_Idle.Checked = False
        CheckBox_GP6_CS_Idle.Enabled = False
        CheckBox_GP6_CS_Active.Checked = False
        CheckBox_GP6_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP6_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP6_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP6_CS_Idle.Checked = False
        CheckBox_GP6_CS_Idle.Enabled = False
        CheckBox_GP6_CS_Active.Checked = False
        CheckBox_GP6_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP6_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP6_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP6_CS_Idle.Enabled = True
        CheckBox_GP6_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP6_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP6_CS_Idle.Click
        If (CheckBox_GP6_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP6_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP6_CS_Active.Click
        If (CheckBox_GP6_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP7_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP7_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP7_CS_Idle.Checked = False
        CheckBox_GP7_CS_Idle.Enabled = False
        CheckBox_GP7_CS_Active.Checked = False
        CheckBox_GP7_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP7_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP7_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP7_CS_Idle.Checked = False
        CheckBox_GP7_CS_Idle.Enabled = False
        CheckBox_GP7_CS_Active.Checked = False
        CheckBox_GP7_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP7_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP7_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP7_CS_Idle.Enabled = True
        CheckBox_GP7_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP7_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP7_CS_Idle.Click
        If (CheckBox_GP7_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP7_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP7_CS_Active.Click
        If (CheckBox_GP7_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    Private Sub RadioButton_GP8_GPIO_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP8_GPIO.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP8_CS_Idle.Checked = False
        CheckBox_GP8_CS_Idle.Enabled = False
        CheckBox_GP8_CS_Active.Checked = False
        CheckBox_GP8_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP8_DedFunc_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP8_DedFunc.Click
        'disable the ACTIVE and IDLE checkboxes
        CheckBox_GP8_CS_Idle.Checked = False
        CheckBox_GP8_CS_Idle.Enabled = False
        CheckBox_GP8_CS_Active.Checked = False
        CheckBox_GP8_CS_Active.Enabled = False
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub RadioButton_GP8_CS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton_GP8_CS.Click
        'enable the ACTIVE and IDLE checkboxes
        CheckBox_GP8_CS_Idle.Enabled = True
        CheckBox_GP8_CS_Active.Enabled = True
        'now call the UpdateGPValues()
        UpdateGPValues()
    End Sub
    '===========================================================
    Private Sub CheckBox_GP8_CS_Idle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP8_CS_Idle.Click
        If (CheckBox_GP8_CS_Idle.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    Private Sub CheckBox_GP8_CS_Active_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox_GP8_CS_Active.Click
        If (CheckBox_GP8_CS_Active.Enabled = True) Then
            'now call the UpdateGPValues()
            UpdateGPValues()
        End If
    End Sub
    '===========================================================
    '===========================================================
    '===========================================================
    Public Sub UpdateGPValues()
        'gather the GP controls values and update the fields

        'get the data from GP0 controls
        If (RadioButton_GP0_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(8) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP0_CS_Idle.Checked = True) Then
                ucIdleCSVal(8) = "1"c
            Else
                ucIdleCSVal(8) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP0_CS_Active.Checked = True) Then
                ucActiveCSVal(8) = "1"c
            Else
                ucActiveCSVal(8) = "0"c
            End If
        ElseIf (RadioButton_GP0_GPIO.Checked = True) Then
            ucPinDes(8) = "0"c
            ucIdleCSVal(8) = "0"c
            ucActiveCSVal(8) = "0"c
        Else
            ucPinDes(8) = "2"c
            ucIdleCSVal(8) = "0"c
            ucActiveCSVal(8) = "0"c
        End If

        'get the data from GP1 controls
        If (RadioButton_GP1_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(7) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP1_CS_Idle.Checked = True) Then
                ucIdleCSVal(7) = "1"c
            Else
                ucIdleCSVal(7) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP1_CS_Active.Checked = True) Then
                ucActiveCSVal(7) = "1"c
            Else
                ucActiveCSVal(7) = "0"c
            End If
        ElseIf (RadioButton_GP1_GPIO.Checked = True) Then
            ucPinDes(7) = "0"c
            ucIdleCSVal(7) = "0"c
            ucActiveCSVal(7) = "0"c
        Else
            ucPinDes(7) = "2"c
            ucIdleCSVal(7) = "0"c
            ucActiveCSVal(7) = "0"c
        End If

        'get the data from GP2 controls
        If (RadioButton_GP2_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(6) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP2_CS_Idle.Checked = True) Then
                ucIdleCSVal(6) = "1"c
            Else
                ucIdleCSVal(6) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP2_CS_Active.Checked = True) Then
                ucActiveCSVal(6) = "1"c
            Else
                ucActiveCSVal(6) = "0"c
            End If
        ElseIf (RadioButton_GP2_GPIO.Checked = True) Then
            ucPinDes(6) = "0"c
            ucIdleCSVal(6) = "0"c
            ucActiveCSVal(6) = "0"c
        Else
            ucPinDes(6) = "2"c
            ucIdleCSVal(6) = "0"c
            ucActiveCSVal(6) = "0"c
        End If

        'get the data from GP3 controls
        If (RadioButton_GP3_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(5) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP3_CS_Idle.Checked = True) Then
                ucIdleCSVal(5) = "1"c
            Else
                ucIdleCSVal(5) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP3_CS_Active.Checked = True) Then
                ucActiveCSVal(5) = "1"c
            Else
                ucActiveCSVal(5) = "0"c
            End If
        ElseIf (RadioButton_GP3_GPIO.Checked = True) Then
            ucPinDes(5) = "0"c
            ucIdleCSVal(5) = "0"c
            ucActiveCSVal(5) = "0"c
        Else
            ucPinDes(5) = "2"c
            ucIdleCSVal(5) = "0"c
            ucActiveCSVal(5) = "0"c
        End If

        'get the data from GP4 controls
        If (RadioButton_GP4_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(4) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP4_CS_Idle.Checked = True) Then
                ucIdleCSVal(4) = "1"c
            Else
                ucIdleCSVal(4) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP4_CS_Active.Checked = True) Then
                ucActiveCSVal(4) = "1"c
            Else
                ucActiveCSVal(4) = "0"c
            End If
        ElseIf (RadioButton_GP4_GPIO.Checked = True) Then
            ucPinDes(4) = "0"c
            ucIdleCSVal(4) = "0"c
            ucActiveCSVal(4) = "0"c
        Else
            ucPinDes(4) = "2"c
            ucIdleCSVal(4) = "0"c
            ucActiveCSVal(4) = "0"c
        End If

        'get the data from GP5 controls
        If (RadioButton_GP5_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(3) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP5_CS_Idle.Checked = True) Then
                ucIdleCSVal(3) = "1"c
            Else
                ucIdleCSVal(3) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP5_CS_Active.Checked = True) Then
                ucActiveCSVal(3) = "1"c
            Else
                ucActiveCSVal(3) = "0"c
            End If
        ElseIf (RadioButton_GP5_GPIO.Checked = True) Then
            ucPinDes(3) = "0"c
            ucIdleCSVal(3) = "0"c
            ucActiveCSVal(3) = "0"c
        Else
            ucPinDes(3) = "2"c
            ucIdleCSVal(3) = "0"c
            ucActiveCSVal(3) = "0"c
        End If

        'get the data from GP6 controls
        If (RadioButton_GP6_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(2) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP6_CS_Idle.Checked = True) Then
                ucIdleCSVal(2) = "1"c
            Else
                ucIdleCSVal(2) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP6_CS_Active.Checked = True) Then
                ucActiveCSVal(2) = "1"c
            Else
                ucActiveCSVal(2) = "0"c
            End If
        ElseIf (RadioButton_GP6_GPIO.Checked = True) Then
            ucPinDes(2) = "0"c
            ucIdleCSVal(2) = "0"c
            ucActiveCSVal(2) = "0"c
        Else
            ucPinDes(2) = "2"c
            ucIdleCSVal(2) = "0"c
            ucActiveCSVal(2) = "0"c
        End If

        'get the data from GP7 controls
        If (RadioButton_GP7_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(1) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP7_CS_Idle.Checked = True) Then
                ucIdleCSVal(1) = "1"c
            Else
                ucIdleCSVal(1) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP7_CS_Active.Checked = True) Then
                ucActiveCSVal(1) = "1"c
            Else
                ucActiveCSVal(1) = "0"c
            End If
        ElseIf (RadioButton_GP7_GPIO.Checked = True) Then
            ucPinDes(1) = "0"c
            ucIdleCSVal(1) = "0"c
            ucActiveCSVal(1) = "0"c
        Else
            ucPinDes(1) = "2"c
            ucIdleCSVal(1) = "0"c
            ucActiveCSVal(1) = "0"c
        End If

        'get the data from GP8 controls
        If (RadioButton_GP8_CS.Checked = True) Then
            'set the pin designation
            ucPinDes(0) = "1"c

            'get the other values as well (IDLE and ACTIVE CS)
            'get the IDLE value
            If (CheckBox_GP8_CS_Idle.Checked = True) Then
                ucIdleCSVal(0) = "1"c
            Else
                ucIdleCSVal(0) = "0"c
            End If

            'get the ACTIVE value
            If (CheckBox_GP8_CS_Active.Checked = True) Then
                ucActiveCSVal(0) = "1"c
            Else
                ucActiveCSVal(0) = "0"c
            End If
        ElseIf (RadioButton_GP8_GPIO.Checked = True) Then
            ucPinDes(0) = "0"c
            ucIdleCSVal(0) = "0"c
            ucActiveCSVal(0) = "0"c
        Else
            ucPinDes(0) = "2"c
            ucIdleCSVal(0) = "0"c
            ucActiveCSVal(0) = "0"c
        End If

        'now update the GP values
        TextBox_PinDes.Text = CStr(ucPinDes)
        TextBox_IdleCSValue.Text = CStr(ucIdleCSVal)
        TextBox_ActiveCSValue.Text = CStr(ucActiveCSVal)
    End Sub
    '===========================================================
    '===========================================================
    '===========================================================
    Private Sub UpdateGPControls()

        ucPinDes = TextBox_PinDes.Text.ToCharArray()
        ucIdleCSVal = TextBox_IdleCSValue.Text.ToCharArray()
        ucActiveCSVal = TextBox_ActiveCSValue.Text.ToCharArray()

        'update GP0 controls
        If (ucPinDes(8) = "0"c) Then
            RadioButton_GP0_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP0_CS_Idle.Enabled = False
            CheckBox_GP0_CS_Active.Enabled = False

        ElseIf (ucPinDes(8) = "1"c) Then
            RadioButton_GP0_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP0_CS_Idle.Enabled = True
            CheckBox_GP0_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(8) = "1"c) Then
                CheckBox_GP0_CS_Idle.Checked = True
            Else
                CheckBox_GP0_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(8) = "1"c) Then
                CheckBox_GP0_CS_Active.Checked = True
            Else
                CheckBox_GP0_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP0_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP0_CS_Idle.Enabled = False
            CheckBox_GP0_CS_Active.Enabled = False
        End If

        'update GP1 controls
        If (ucPinDes(7) = "0"c) Then
            RadioButton_GP1_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP1_CS_Idle.Enabled = False
            CheckBox_GP1_CS_Active.Enabled = False

        ElseIf (ucPinDes(7) = "1"c) Then
            RadioButton_GP1_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP1_CS_Idle.Enabled = True
            CheckBox_GP1_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(7) = "1"c) Then
                CheckBox_GP1_CS_Idle.Checked = True
            Else
                CheckBox_GP1_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(7) = "1"c) Then
                CheckBox_GP1_CS_Active.Checked = True
            Else
                CheckBox_GP1_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP1_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP1_CS_Idle.Enabled = False
            CheckBox_GP1_CS_Active.Enabled = False
        End If

        'update GP2 controls
        If (ucPinDes(6) = "0"c) Then
            RadioButton_GP2_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP2_CS_Idle.Enabled = False
            CheckBox_GP2_CS_Active.Enabled = False

        ElseIf (ucPinDes(6) = "1"c) Then
            RadioButton_GP2_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP2_CS_Idle.Enabled = True
            CheckBox_GP2_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(6) = "1"c) Then
                CheckBox_GP2_CS_Idle.Checked = True
            Else
                CheckBox_GP2_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(6) = "1"c) Then
                CheckBox_GP2_CS_Active.Checked = True
            Else
                CheckBox_GP2_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP2_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP2_CS_Idle.Enabled = False
            CheckBox_GP2_CS_Active.Enabled = False
        End If

        'update GP3 controls
        If (ucPinDes(5) = "0"c) Then
            RadioButton_GP3_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP3_CS_Idle.Enabled = False
            CheckBox_GP3_CS_Active.Enabled = False

        ElseIf (ucPinDes(5) = "1"c) Then
            RadioButton_GP3_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP3_CS_Idle.Enabled = True
            CheckBox_GP3_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(5) = "1"c) Then
                CheckBox_GP3_CS_Idle.Checked = True
            Else
                CheckBox_GP3_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(5) = "1"c) Then
                CheckBox_GP3_CS_Active.Checked = True
            Else
                CheckBox_GP3_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP3_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP3_CS_Idle.Enabled = False
            CheckBox_GP3_CS_Active.Enabled = False
        End If

        'update GP4 controls
        If (ucPinDes(4) = "0"c) Then
            RadioButton_GP4_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP4_CS_Idle.Enabled = False
            CheckBox_GP4_CS_Active.Enabled = False

        ElseIf (ucPinDes(4) = "1"c) Then
            RadioButton_GP4_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP4_CS_Idle.Enabled = True
            CheckBox_GP4_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(4) = "1"c) Then
                CheckBox_GP4_CS_Idle.Checked = True
            Else
                CheckBox_GP4_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(4) = "1"c) Then
                CheckBox_GP4_CS_Active.Checked = True
            Else
                CheckBox_GP4_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP4_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP4_CS_Idle.Enabled = False
            CheckBox_GP4_CS_Active.Enabled = False
        End If

        'update GP5 controls
        If (ucPinDes(3) = "0"c) Then
            RadioButton_GP5_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP5_CS_Idle.Enabled = False
            CheckBox_GP5_CS_Active.Enabled = False

        ElseIf (ucPinDes(3) = "1"c) Then
            RadioButton_GP5_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP5_CS_Idle.Enabled = True
            CheckBox_GP5_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(3) = "1"c) Then
                CheckBox_GP5_CS_Idle.Checked = True
            Else
                CheckBox_GP5_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(3) = "1"c) Then
                CheckBox_GP5_CS_Active.Checked = True
            Else
                CheckBox_GP5_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP5_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP5_CS_Idle.Enabled = False
            CheckBox_GP5_CS_Active.Enabled = False
        End If

        'update GP6 controls
        If (ucPinDes(2) = "0"c) Then
            RadioButton_GP6_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP6_CS_Idle.Enabled = False
            CheckBox_GP6_CS_Active.Enabled = False

        ElseIf (ucPinDes(2) = "1"c) Then
            RadioButton_GP6_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP6_CS_Idle.Enabled = True
            CheckBox_GP6_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(2) = "1"c) Then
                CheckBox_GP6_CS_Idle.Checked = True
            Else
                CheckBox_GP6_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(2) = "1"c) Then
                CheckBox_GP6_CS_Active.Checked = True
            Else
                CheckBox_GP6_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP6_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP6_CS_Idle.Enabled = False
            CheckBox_GP6_CS_Active.Enabled = False
        End If

        'update GP7 controls
        If (ucPinDes(1) = "0"c) Then
            RadioButton_GP7_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP7_CS_Idle.Enabled = False
            CheckBox_GP7_CS_Active.Enabled = False

        ElseIf (ucPinDes(1) = "1"c) Then
            RadioButton_GP7_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP7_CS_Idle.Enabled = True
            CheckBox_GP7_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(1) = "1"c) Then
                CheckBox_GP7_CS_Idle.Checked = True
            Else
                CheckBox_GP7_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(1) = "1"c) Then
                CheckBox_GP7_CS_Active.Checked = True
            Else
                CheckBox_GP7_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP7_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP7_CS_Idle.Enabled = False
            CheckBox_GP7_CS_Active.Enabled = False
        End If

        'update GP8 controls
        If (ucPinDes(0) = "0"c) Then
            RadioButton_GP8_GPIO.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP8_CS_Idle.Enabled = False
            CheckBox_GP8_CS_Active.Enabled = False

        ElseIf (ucPinDes(0) = "1"c) Then
            RadioButton_GP8_CS.Checked = True
            'enable the checkboxes
            CheckBox_GP8_CS_Idle.Enabled = True
            CheckBox_GP8_CS_Active.Enabled = True

            'now update the checkboxes
            If (ucIdleCSVal(0) = "1"c) Then
                CheckBox_GP8_CS_Idle.Checked = True
            Else
                CheckBox_GP8_CS_Idle.Checked = False
            End If

            If (ucActiveCSVal(0) = "1"c) Then
                CheckBox_GP8_CS_Active.Checked = True
            Else
                CheckBox_GP8_CS_Active.Checked = False
            End If

        Else
            RadioButton_GP8_DedFunc.Checked = True
            'disable the IDLE and ACTIVE checkboxes
            CheckBox_GP8_CS_Idle.Enabled = False
            CheckBox_GP8_CS_Active.Enabled = False
        End If
    End Sub
    '===========================================================
End Class
