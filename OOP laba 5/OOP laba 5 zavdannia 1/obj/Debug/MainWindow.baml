   M S B A M L   `   `   `  ����Q  MOOP laba 5 zavdannia 1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:HorseRaceSimulatorHorseRaceSimulator  5   	   #    HorseRaceSimulator.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:HorseRaceSimulator     5   	     � Title$  Task1��5   	    � WindowState$ 	Maximized=�5	   	    � 
ResizeMode$ 	CanResize=�5
   	    � Topmost .   � 	Resources 5         dataGridStyle �� 
TargetType&   System.Windows.Controls.DataGrid �� Property �� Value��%�  &	        �� 5   
   "  6&   .�5      ��  � ��
 �����6+   �� 5       �@ RowBackground
 �����6.   �� 5        �@ AlternatingRowBackground
 ��ح�69   �� 5        �@ HorizontalGridLinesBrush
 �����69   �� 5       �@ VerticalGridLinesBrush
 �����67   �� 5       � ��$ 14(�6)   �� 5       �@ CanUserAddRows .  6/   �� 5       �@ CanUserResizeColumns . 65   �� 5       �@ 
IsReadOnly . 6+   5      5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��6* �6   	 � RowDefinitions	 5   
   �� 5       $��4* �6   �� 5!      $��2* �6   .+�5#   
   �� -   5        $��canvas��5#      $��1��6   $��0��6/   
������ .+�5$      �� 
 �� Left$
 0��5'      
 �� Top$ 0��6    $
��Fill=�5(      $!��Images\Background\Track.png��5)      ��5%      �� *ActualWidth+ �� ElementName$ canvas����5&      �� *ActualHeight+$ canvas��5)      5*        5,   
   -   5        $��dataGrid��5,      $��1��6#   $��1��63   #��[  5-         AutoGenerateColumns .     
IsReadOnly . ��5.      �� *Horses+   Columns 51      4  .System.Windows.Controls.DataGridTemplateColumn  52      ,  &System.Windows.Controls.DataGridColumn   Header$ Color��6)      CellTemplate 53      �� 54      .R�55      ��  �� Orientation$ 
Horizontal=�6)   .+�56   "   A� $��20��6A   $��20��6L   $��0,0,5,0q�6X   ��6*   �� *Color+6X   57      58      5:      0  *System.Windows.Controls.DataGridTextColumn  5;      $
 Name��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *Name+  5<      $ Position��6%    67   �� *
Position+  5=      $
 Time��6%    63   �� *Timer+  5>      $ Coefficient��6%    6:   �� *Coefficient+  5?      $ Money��6%    64   �� *Money+5A      �� 5B   
   $ Vertical=�6   $��20q�6,   $	��200��68   
�������$��Center=�6W   .+�5D      �� $��Bettings��6   $��20(�5E      
��� ���$
��Bold"�5G       �� TextAlignment$ Center=�5H      $��0,10,0,5q�5I      �� 5K      �� 5N      $��14(�5O      $��0,10,0,5q�5P      $��Center=�5Q      ��5N      �� *BalanceText+5S      �� 5V      $ 
Horizontal=�6   $��Center=�62   $	��0,5q�6O   .+�5X      �� -   5        $��60��5Y      $��Center=�5Z      $��14(�5[      $
��Bold"�5\      
��� ���$
��10,0q�5^       �� TextAlignment$ Center=�5`      ��5X      �� *	BetAmount+ �� UpdateSourceTrigger$ PropertyChanged=�5`      5a      �� 5c      $��bet on the horse��6   $��12(�5d      
�������$��Center=�5f      $	��0,5q�5g      �� 5j      $ 
Horizontal=�6   $��Center=�62   .+�5k      �� -   5        $��<��5k      $��30��6(   $��25��63   6?   �� 5l      
��� ���$
��Bold"�5n      $
��10,0q�5o      ��5l      �� *SelectedHorseText+5o      �� 5p      -   5        $��>��5p      $��30��6(   $��25��63   6?   5q      �� 5t      -   5        $	��BET��5t      $��0,10,0,0q�5u      $��30��5v      $	��100��5w      $��Center=�5x      
�������
���   �5{      �� 5|      -   5        $��
Start Race��5|      $��0,10,0,0q�5}   	   $��30��5~   	   $	��100��5   	   $��Center=�5�   	   
������5�   	   �� 5�      $��Select horses:��6   $
��Bold"�5�      
���   �$��0,10,0,5q�5�      $��Center=�5�      �� 5�      $ Vertical=�6   $��10,0,0,0q�60   .+�5�      �� -   5         ��Name$ 
CheckAnton��5�      $��Anton��6/   $	��5,2q�6?   �� 5�      -	   5        $ 	CheckDima��5�      $
��Dima��6.   $	��5,2q�6=   �� 5�      -
   5        $ 	CheckIvan��5�      $
��Ivan��6.   $	��5,2q�6=   �� 5�      -   5        $ 
CheckArtem��5�      $��Artem��6/   $	��5,2q�6?   �� 5�      -   5        $ 
CheckSasha��5�      $��Sasha��6/   $	��5,2q�6?   �� 5�      -   5        $ 	CheckAnna��5�      $
��Anna��6.   $	��5,2q�6=   �� 5�      -   5        $ 	CheckOleg��5�      $
��Oleg��6.   $	��5,2q�6=   �� 5�      -   5        $ CheckMax��5�      $	��Max��6-   $	��5,2q�6;   �� 5�      -   5        $ 	CheckVova��5�      $
��Vova��6.   $	��5,2q�6=   �� 5�      -   5        $ 	CheckIgor��5�      $
��Igor��6.   $	��5,2q�6=   5�      5�      5�      5�      