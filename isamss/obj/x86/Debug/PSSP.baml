   M S B A M L   `   `   `  ����A  =isamss, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:isamssisamss  5          isamss.PSSPForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      $srcclr-namespace:isamss     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   +     � Title$
  PSSP��6   $	��503��6   $	��396��6    � Icon$&  /isamss;component/Images/add.png��6d   .��5      � .+�5   
   +  %Microsoft.Windows.Controls.DatePicker  -   5        $��dtOriginationDate��5   F   $
��Left=�6   $��	12,31,0,0q�63   $	��Top=�6_   $��96��6w   $��0��6�   �� 5   
   -   5        $��Label6��5   b   $��Effective Date��6   $��25��6)   $
��Left=�65   $��10,6,0,0q�6P   $	��Top=�6p   �� 5	   
   -   5        $��btn_save��5	   \   $
��Save��6   $��23��6    $
��Left=�6,   $��286,429,0,0q�6G   $��4��6l   $	��Top=�6y   $��75��6�   {�.  6�   �� 5
   
   -   5        $��
btn_cancel��5
   ^   $��Cancel��6   $��23��6"   $
��Left=�6.   $��205,429,0,0q�6I   $��5��6p   $	��Top=�6}   $��75��6�   �� 5   
   -   5        $��txtNotes��5   M   $��76��6   $
��Left=�6   $��
12,159,0,0q�69   $	��Top=�6]   $	��349��6u    �� AcceptsReturn . $��3��6�    �� TextWrapping$
 Wrap=�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�   �� 5   
   -   5        $��Label2��5   [   $��Notes��6   $��25��6    $
��Left=�6,   $��
12,137,0,0q�6G   $	��Top=�6i   �� 5   
   -   5        $��cboActionClasses��5   N   $��23��6   $
��Left=�6   $��
205,31,0,0q�6:   $	��Top=�6f   $	��156��6~   ��6�   ��  �� Path$ Title��� 5   
   -   5        $��lblActionType��5   c   $��Type of Action��6   $��25��6)   $
��Left=�65   $��	205,6,0,0q�6P   $	��Top=�6x   "   isamss.TStackPanelAttachment  5   
   -	   5          Name$ stpAttachment��5   R   $
��Left=�6$   $��	12,59,0,0q�6?   $	��Top=�6i   �� 5   
   -
   5        $��	GroupBox1��5   g   $��Change History��6   $	��173��6+   $
��Left=�68   $��
12,241,0,0q�6S   $	��Top=�6x   $	��349��6�   .��5      � .+�5      �� -   5        $��lstvwPSSPHistory��5   l   $	��113��6   $
��Left=�6(   $��6,6,0,0q�6[   $	��Top=�6�   $	��325��6�   $��2��6�   {�. ��6C   ��  � ContextMenu 5      �� 5      -   5        $��psspContextMenu��5   &   .@�5      m� -   5        $��MenuItemDeleteHistoryItem��5   E   $��_Delete History Item��6'    m� Icon 5   "   �� 5   &   $)��#/isamss;component/Images/delete.png��6,   5      5      	 �� View	 5      �� 5      
 �� AllowsColumnReorder
 .  .P�5      �� $��Action Taken��6l    �� DisplayMemberBinding 6-   �� $ HistoryActionClass.Title�6l   �� 5      $
��User��6a    6-   �� $ User.FullName�6a   �� 5       $
��Date��6n    6-   �� $ 
ActionDate� �� StringFormat$ d��6n   5!      5#      �� 5$      -   5        $��btnAddPSSPHistory��5$   u   $	��Add��6   $��2��6'   $��23��6;   $
��Left=�6G   $��	6,126,0,0q�6b   $	��Top=�6�   $��75��6�   {�. 6�   5%      5&      �� 5'   
   -   5        $��Label1��5'   _   $��
Attachment��6   $��23��6%   $
��Left=�61   $��	12,70,0,0q�6L   $	��Top=�6m   $	��106��6�   5(      5)      