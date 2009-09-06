\ ==============================================================================
\
\       gsv13_expl - the gtk-server entry example in the ffl
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


  0 value window                  \ Widget \ events
  0 value entry
  0 value editable
  0 value visibility
  0 value exit-button


  : example
    gtk_init                      \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    100 200 window gtk_widget_set_size_request
    
    s" GTK Entry" window gtk_window_set_title

    0 false gtk_vbox_new >r
    
    r@ window gtk_container_add
    
    gtk_entry_new to entry
    
    50 entry gtk_entry_set_max_length
    
    s" hello" entry gtk_entry_set_text
    
    entry gtk_entry_get_text nip
    -1 s" world" entry gtk_editable_insert_text drop
    
    entry gtk_entry_get_text nip
    0 entry gtk_editable_select_region

    0 true true entry r@ gtk_box_pack_start
    
    entry gtk_widget_show
    

    0 false gtk_hbox_new >r
    
    r@ r'@ gtk_container_add
                                 
    s" Editable" gtk_check_button_new_with_label to editable
    
    0 true true editable r@ gtk_box_pack_start
    
    true editable gtk_toggle_button_set_active
    
    editable gtk_widget_show
    
    
    s" Visible" gtk_check_button_new_with_label to visibility
    
    0 true true visibility r@ gtk_box_pack_start
    
    true visibility gtk_toggle_button_set_active
    
    visibility gtk_widget_show
    
    r> gtk_widget_show            \ hbox
    
    s" gtk-close" gtk_button_new_from_stock to exit-button
    
    0 true true exit-button r@ gtk_box_pack_start
    
    \ GTK_WIDGET_SET_FLAGS (button, GTK_CAN_DEFAULT);
    \ gtk_widget_grab_default (button);
    
    exit-button gtk_widget_show
    
    r> gtk_widget_show            \ vbox
    
    window gtk_widget_show
    
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback  
    
      event>widget CASE
        window       OF true ENDOF
        exit-button  OF true ENDOF
        entry        OF ." Entry contents: " entry gtk_entry_get_text type cr  
                        0 0 entry gtk_editable_get_selection_bounds 
                        ." Selection: " rot . ." Start:" swap . ." End:" . cr  false ENDOF
        editable     OF editable   gtk_toggle_button_get_active  entry gtk_editable_set_editable  false ENDOF
        visibility   OF visibility gtk_toggle_button_get_active  entry gtk_entry_set_visibility   false ENDOF
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
