   M S B A M L   `   `   `  ����S  OFileShuffler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8e68b26b53b9f87c!clr-namespace:RenamerRenamer  5      T PMahApps.Metro, Version=1.4.3.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fD2clr-namespace:MahApps.Metro;assembly=MahApps.MetroMahApps.Metro 5      V;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls 5          Renamer.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      'localclr-namespace:Renamer     5      Dmetro2clr-namespace:MahApps.Metro;assembly=MahApps.Metro     5      PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5        � Title$  fileshuffler IV��5
      $	��500��5      $	��900��5      (  "MahApps.Metro.Controls.MetroWindow   ShowMaxRestoreButton .   � 
ResizeMode$ CanResizeWithGrip=�5         AccentColorBrush   	GlowBrush# �   5         WindowTransitionsEnabled .   � WindowStartupLocation$ CenterScreen=�5         RightWindowCommands 5      +  %MahApps.Metro.Controls.WindowCommands  5   
   .@�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ nummer��5      �� 5      $��0,0,8,0q�6   $��Center=�5      $��Change Accent Colour��5      �� 5      -   5        	 ��Name$	 AccentSelector��5      $��0 0 2 0q�5      $��Center=�5         MahApps.Metro.ThemeManager
   Accents#��Z
 5      $��90��5      ��5      �� 5       .R�5!      �� ��6(   �� *Name+5"      5$      5%      5&       � 	Resources 5(      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value �� Color��%�   '        �� 5)   
   "  6   .�5*      ��  � �� 5+      �� 5,      $ 	#FFE8E2D8��6&   5.      �� 5/       � ��$ 16(�50      �� 51       � ��$ DemiBold"�52      53      54      .��55      � $��0,0,0,0q�6   ���	#FFE8E2D8 � RowDefinitions 58   
   �� 59      $��54 �6   �� 5:      $	��77* �6   �� 5;      $��55 �6   �� 5<      $��25 �6   .+�5?   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5?      $
��Left=�5@      $��1��5A      $	��Top=�5B      $y�Visible=�5C         AutoGenerateColumns .   J� 	AllowDrop . $��14(�5I        AzureDataGrid#��[ 5J         CanUserAddRows .  $��
10,10,10,0q�5M      ���	#FFE8E2D8��5L      �� *
ViewSource+   Columns 5P      0  *System.Windows.Controls.DataGridTextColumn  5R      ,  &System.Windows.Controls.DataGridColumn   Header$ Part Number��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$
 1.3* 5T   %   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 5S   %   �� *PartNo+5T   %     5U      $ Description��6%   $
 1.3* 5W   %    5V   %   �� *FileDescription+5W   %     5X      $ Filetype��6%   $
 0.9* 5Z   %    5Y   %   �� *	Extension+5Z   %     5[      $ 	File Size��6%   $ 1* 5]   %    5\   %   �� *
FileSize+5]   %   4
  .System.Windows.Controls.DataGridCheckBoxColumn
  5^        MetroDataGridCheckBox 	  ElementStyle# �  6)    	  EditingElementStyle# �  5_   )   $ Copy status��5`   )   $
 1.2* 5b   )    5a   )   �� *	SiteFound+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5b   )     5c      $ Supplier��6%   $
 1.2* 5e   %    5d   %   �� *
Supplier+5e   %     5f      $ Version��6%   $
 0.9* 5h   %    5g   %   �� *	Version+$ TwoWay=�$ PropertyChanged=�5h   %     5i      $ Status��6%   $ 1* 5k   %    5j   %   �� *Status+$ TwoWay=�$ PropertyChanged=�5k   %     5l      $ sourcelocation��6%      
Visibility$ 	Collapsed=�5n   %    5m   %   �� *SourceLocation+5n   %   4  .System.Windows.Controls.DataGridTemplateColumn  5o      $
 Link��6*   $
 0.8* 5p   *      CellTemplate 5r      �� 5s      .R�5t      ��  ��Name$ 
LinkButton��6%   -   5          AccentedSquareButtonStyle#��[ 5w   %   $��32��5x   %   $
��Link��5y   %   ��5v   %   �� *
FolderName+5y   %   5{      5}      5�      �� 5�   
   $��3��6   $��35��5�      $��Bottom=�5�      .@�5�      �� $��13.333(�6   $
��Left=�5�      $	��692��5�      .��5�      �� -   5          ��Name$  StatusIndicator��5�      $	��444��5�      $�� ��5�      5�      5�      �� 5�   
   -   5        $ clear_button��5�      $��Clear Content��5�      #��[ 5�      $
��Left=�5�      $��2��5�      $��Center=�5�      $	��115��5�      $��32��5�      $	��120��5�      $	��120��5�      $��25��5�      $��32��5�      $��30,0,0,0q�5�      �� 5�   
   -   5        $ send_button��5�      #��[ 5�      $��Send to supplier sites��5�      $��2��5�      $��Center=�5�      $��Right=�5�      $��32��5�      $	��200��5�      $	��200��5�      $��25��5�      $��32��5�      $��0,0,30,0q�5�      )  #MahApps.Metro.Controls.ProgressRing  5�   
   -   5        !  Name$! MyProgressRing��5�       "   IsActive" .  $��Center=�5�       $��Center=�5�       $	��100��5�       $	��100��5�       $��1��5�       $��296,102,296,106q�5�       $y�	Collapsed=�5�       �� 5�   
   $��NoWrap=�6   $'��!drop your files below to add them��5�      $��24(�5�      # J� RenderTransformOrigin$# 0.597,0.505&�5�      $
��Left=�5�      $	��480��5�      $��	10,4,0,15q�5�      $��0��5�      �� 5�   
   -	   5        $ button��5�      $
��help��5�      #��[ 5�      $��Right=�5�      $��607,13,10,10q�5�      $	��Top=�5�      $��75��5�      $��0��5�      5�      �� 5�   
   -
   5        $ ��Name$$ 	dropimage��5�      $��Center=�5�      $��1��5�      $��265,75,264,72q�5�      $��Center=�5�      $��download_grey.png��5�      $��UniformToFill=�5�      $	��128��5�      $	��128��5�      % J� Opacity$
% 0.15Q�5�       . 5�      5�      5�      