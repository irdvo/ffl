\ ==============================================================================
\
\       gsv17_expl - the gtk-server file chooser example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/gsv.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : example
    gtk_init                      \ Initialise toolkit
                                  \ Open file chooser dialog
    GTK_RESPONSE_ACCEPT s" gtk-open" GTK_RESPONSE_CANCEL s" gtk-cancel" GTK_FILE_CHOOSER_ACTION_OPEN 0 s" Open File" gtk_file_chooser_dialog_new >r
    
    r@ gtk_dialog_run  GTK_RESPONSE_ACCEPT = IF
      ." Open file:" r@ gtk_file_chooser_get_filename type cr
    THEN
    
    r> gtk_widget_destroy

    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
