#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.ConstrainedExecution
Imports System.Security.Permissions

Imports Microsoft.Win32.SafeHandles

#End Region

#Region " Safe Window Handle "

Namespace Win32.SafeHandles

    ''' <summary>
    ''' Represents a safe Win32 window handle (hWnd).
    ''' </summary>
    <SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode:=True)>
    Friend NotInheritable Class SafeWindowHandle : Inherits SafeHandleZeroOrMinusOneIsInvalid

        ''' <summary>
        ''' Determines whether this is a valid handle.
        ''' </summary>
        Overrides ReadOnly Property IsInvalid As Boolean
            Get
                Return Me.handle.ToInt32() <= 0
            End Get
        End Property

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SafeWindowHandle"/> class.
        ''' </summary>
        Friend Sub New()
            MyBase.New(ownsHandle:=True)
            Me.SetHandle(IntPtr.Zero)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SafeWindowHandle"/> class.
        ''' </summary>
        ''' <param name="hWnd">A handle to the window to wrap.</param>
        Friend Sub New(hWnd As IntPtr)
            MyBase.New(ownsHandle:=True)
            Me.SetHandle(hWnd)
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Releases the handle.
        ''' </summary>
        ''' <returns><see langword="True"/> if handle is released successfully, <see langword="False"/> otherwise.</returns>
        <ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)>
        Protected Overrides Function ReleaseHandle() As Boolean
            Return (NativeMethods.DestroyWindow(Me.handle) = IntPtr.Zero)
        End Function

#End Region

    End Class

End Namespace

#End Region
