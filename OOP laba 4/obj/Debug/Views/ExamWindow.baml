   M S B A M L   `   `   `  ����  
OOP laba 4!    OOP_laba_4.Views.ExamWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$#  Редактор іспиту��5   	    � SizeToContent$ WidthAndHeight=�6!    � WindowStartupLocation$ CenterScreen=�6@   .��5      � $��15q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $	��200 �6   .+�5   
   �� $��0��6   $��0��6   $"��Назва предмету:��6-   $��Center=�6G   �� 5   
   -   5         ��Name$ SubjectNameTextBox��5      $��0��6.   $��1��6;   $��5q�6K   �� 5   
   $��1��6   $��0��6   $��Оцінка (0-100):��6-   $��Center=�6G   �� 5   
   -   5        $ GradeTextBox��5      $��1��6(   $��1��65   $��5q�6E   �� 5   
   $��2��6   $��0��6   $"��Дата складання:��6-   $��Center=�6G   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ ExamDatePicker��5      $��2��6-   $��1��6:   $��5q�6J   �� 5   
   $��3��6   $��2��6"    �� Orientation$ 
Horizontal=�66   $��Right=�6O   $��0,15,0,0q�6k   .+�5      �� -   5        $��Зберегти��5      $	��100��6(   $��5q�64    �� 	IsDefault . 6P   �� 5      -   5        $��Скасувати��5      $	��100��6)   $��5q�65   	 �� IsCancel	 . 6P   5      5      5       