\ ==============================================================================
\
\         gsv5_expl - the gtk-server pixmap example in the ffl
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

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  : xpm_label_box  ( c-addr1 u1 c-addr2 u2 -- box = Create a box with a image c-addr2 u2 and label c-addr1 u1 )
  
    0 false gtk_hbox_new >r       \ Create box for image and label
    
    2 r@ gtk_container_set_border_width
    
    
    gtk_image_new_from_file >r    \ Now on to the image stuff
    
    3 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    gtk_label_new >r              \ Create a label for the button
    
    3 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    r>
  ;
    
  0 value window                \ the event widgets
  0 value button
  0 value enter-button

  : example
    gtk_init                      \ Initialise toolkit
                                  \ Create toplevel window and connect delete-event with word
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Pixmap'd Buttons!" window gtk_window_set_title

    10 window gtk_container_set_border_width

    gtk_button_new to button

    16 g_malloc to enter-button   \ Generate unique address\widget
    
                                  \ Connect button with event enter to enter-button
    enter-button s" enter" button gsv+server-connect 2drop
    
    s" cool button" s" info.xpm" xpm_label_box >r

    r@ gtk_widget_show            \ xpm label box

    r> button gtk_container_add

    button gtk_widget_show       \ button

    button window gtk_container_add

    window gtk_widget_show        \ window
                                  \ Main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        button       OF ." Hello again - button was pressed" cr false ENDOF
        enter-button OF ." Hello again - button was entered" cr false ENDOF
        false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
