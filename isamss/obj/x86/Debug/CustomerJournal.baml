   M S B A M L   `   `   `  ����A  =isamss, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:isamssisamss  5   �       isamss.CustomerJournal   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   I   my1clr-namespace:isamss  6�     � Title$(  "Customer Interaction Journal Entry��6   $	��383��60   $	��394��6=    � Icon$( "/isamss;component/Images/group.png��6�   .��5      � .+�5   
   +  %Microsoft.Windows.Controls.DatePicker  -   5        $��dtEntryDate��5   G   $
��Left=�6   $��
252,22,0,0q�63   $	��Top=�6Z   $	��109��6r   $��24��6~   $��0��6�   �� 5   
   -   5        $��Label1��5   _   $��
Entry Date��6   $��24��6%   $
��Left=�61   $��	252,2,0,0q�6L   $	��Top=�6m   $	��109��6�   �� 5   
   -   5        $��txtCustomer��5   e   ���	#FFE8EAF5$��18��6+   $
��Left=�67   $��	12,26,0,0q�6R   $�� ��6x   $	��Top=�6�   $	��220��6�   �� 5	   
   -   5        $��Label2��5	   \   $��Customer��6   $��24��6#   $
��Left=�6/   $��12,3,0,0q�6J   $	��Top=�6j   $	��109��6�   �� 5
   
   -   5        $��txtInteractionNotes��5
   M   $	��158��6   $
��Left=�6   $��	12,78,0,0q�6:   $	��Top=�6h   $	��349��6�   $��1��6�    �� AcceptsReturn .  �� TextWrapping$
 Wrap=�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�   �� 5   
   -   5        $��Label3��5   f   $��Interaction Notes��6   $��24��6,   $
��Left=�68   $��	12,56,0,0q�6S   $	��Top=�6t   $	��109��6�   "   isamss.TStackPanelAttachment  5   
   -   5          Name$ tspAttachment��5   S   $
��Left=�6$   $��
12,236,0,0q�6?   $	��Top=�6j   $��2��6�   �� 5   
   -   5        $��	btnCancel��5   ^   $��Cancel��6   $��23��6"   $
��Left=�6.   $��205,315,0,0q�6I   $	��Top=�6o   $��75��6�   $��4��6�   �� 5   
   -	   5        $��btnSave��5   \   $
��Save��6   $��23��6    $
��Left=�6,   $��286,315,0,0q�6G   $	��Top=�6k   $��75��6�   $��3��6�   �� 5   
   -
   5        $��Label4��5   `   $��
Attachment��6   $��28��6%   $
��Left=�61   $��
12,245,0,0q�6L   $	��Top=�6n   $	��106��6�   5      5      