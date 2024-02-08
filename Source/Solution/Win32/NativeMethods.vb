#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Runtime.InteropServices

Imports Win32.SafeHandles

#End Region

#Region " P/Invoking "

Namespace Win32

    ''' <summary>
    ''' Platform Invocation methods (P/Invoke), access unmanaged code.
    ''' <para></para>
    ''' This class does not suppress stack walks for unmanaged code permission.
    ''' <see cref="Security.SuppressUnmanagedCodeSecurityAttribute"/> must not be applied to this class.
    ''' <para></para>
    ''' This class is for methods that can be used anywhere because a stack walk will be performed.
    ''' </summary>
    ''' <remarks>
    ''' <see href="https://msdn.microsoft.com/en-us/library/ms182161.aspx"/>
    ''' </remarks>
    Friend NotInheritable Class NativeMethods

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
        ''' </summary>
        <DebuggerNonUserCode>
        Private Sub New()
        End Sub

#End Region

#Region " P/Invokes "

        <DllImport("kernel32.dll", SetLastError:=False)>
        Friend Shared Function GetConsoleWindow() As SafeWindowHandle
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function DestroyWindow(hWnd As SafeWindowHandle) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function DestroyWindow(hWnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function SetLayeredWindowAttributes(hWnd As SafeWindowHandle,
                                                          crKey As UInteger,
                                                          bAlpha As Byte,
                                                          dwFlags As UInteger) As Boolean
        End Function

#End Region

    End Class

End Namespace

#End Region
