   M S B A M L   `   `   `  ����K  GhapControlGUIApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:hapControlGUIApphapControlGUIApp  5   	   '    !hapControlGUIApp.ProcessingSplash   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:hapControlGUIApp     5   	     � Title$  ProcessingSplash��5   	   $	��300��5	   	   $	��110��5
   	    � WindowStyle$ SingleBorderWindow=�5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��30 �6   .+�5   
   � -   5         �Name$ progressbar��5      $��Center=�5      $��Stretch=�5      $��20��5      $��
20,10,24,1q�5      $	��250��5       � IsIndeterminate . $��1��5   -   �� 5   
   -   5         ��Name$ label_status��5      $��Center=�5      $��
17,8,267,2q�6.   �� 5   
   -   5        $ label��5      $
��Left=�63   $��32,3,0,0q�6N   $	��Top=�6`   ��6   �� 6`   5      5       