   M S B A M L   `   `   `  ????H  DsteamcmdTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:steamcmdsteamcmd  5   	       steamcmd.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:steamcmd     5   	   -   5          ?Name$  DestinyMainWindow??5   	    ? Title$" Destiny-STEAMCMD下载工具??5   	   $	??302??6&   $	??809??63    ? 
ResizeMode$ NoResize=?6?    ? Icon$ /Resources/IC.ico??6U   .??5	      ? $??0,5,0,1q?6    ? ColumnDefinitions 5
   
   ?? 5      $??6* ?6   ?? 5      $	??74* ?6   ?? 5      $	??98* ?6   ?? 5      $
??133* ?6   ?? 5      $
??121* ?6   ?? 5      $
??138* ?6   ?? 5      $
??106* ?6   ?? 5      $	??70* ?6   ?? 5      $	??40* ?6   ?? 5      $	??14* ?6   .+?5   
   ?? -   5         ??Name$ TextBoxOutput??5      $
??Left=?6)    ?? TextWrapping$
 Wrap=?6D   $??	载入...??6X   $	??Top=?6e   $	??780??6}   $	??191??6?   $??9??6?    ?? AcceptsReturn$
 True??6?   " ?? VerticalScrollBarVisibility$ Visible=?6?   	 ?? 
IsReadOnly$
	 True??6?   ???	#FFAF9393
???????$??4,58,0,0q?6!  $??1??63  $??8(?6C  ? 5   
   -   5        
 ?Name$
 DownloadProgressBar??5      $??1??63   $??9??6C   $
??Left=?6W   $??19??6r   $??4,28,0,0q?6~   $	??Top=?6?   $	??780??6?   ?? 5   
   -   5         ??Name$ 	LabelText??5      $??1??6#   $??	等待...??63   $
??Left=?6C   $??4,5,0,0q?6^   $	??Top=?6o   $	??403??6?   $??4??6?   ?? 5   
   -   5        $ LabelText_Sum??5      $??6??6'   $??--??67   $??Right=?6D   $??0,5,5,0q?6`   $	??Top=?6q   $	??128??6?   $??3??6?   5      5      