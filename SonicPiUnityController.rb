'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo María Cidoncha Pérez and Mario Tabasco Vargas
Version 0.0
'''

require 'ostruct'

'''
============================
  Command Struct
============================
Parameters:
 - loop_id: The index of the loop to which the command is attached
 - com_name: The string containing the name of the command
 - com_id: The index of the command inside the loop command list (-1 = new command)
 - com_attr: List of attributes (pairs of <name, value>) of the command (i.e. <amp, 0.7>)

'''

Command = Struct.new(:loop_id, :com_name, :com_id, :com_attr) do
  # Initialize
  
  # Getters
  def getName
    com_name
  end
  
  def getId
    loop_id
  end
  
  def getAttributes
    com_attr
  end
  
end

'''
  ============================
    Methods
  ============================
  '''

'''
  Waits for messages from Unity and adds the received commands
  - id: Index of the listening loop
  '''
def listenUnityCommand(id, commands)
  # Gets OSC message from Unity
  val = sync"/osc*/sonicpi/unity/trigger"
  puts "Received command " + val[1]
  
  # Parse message to Command
  cLoopId = val[0]
  cComName = val[1]
  cComId = val[2]
  cComAttr = val[3]
  
  if cLoopId == -1 # New command
    puts "new command"
  elsif
    puts "change command attributes"
  end
  
  
  #com = Command.new
  
  # Add the command to command list
  commands.push(val)
  puts commands
end

'''
  Process the command list
  - id: Index of the processing loop
  '''
def processCommands(id, commands)
  # Processes each command from the command list in order
  commands.each do |com|
    puts "Processing: " + com[1]
    case com[0]
    when "sleep"
      sleep 1
    when "play"
      play 89, amp: 0.1
    when "stop"
      stop
    else
      puts "ERROR"
    end
  end
  sleep 0.1
end


'''
  ============================
    Listen/Play loops definition
  ============================
  '''

def doListenerLoop(id,commands)
  puts "Listener " + id.to_s
  # LISTENER LOOP
  in_thread do
    loop do
      #puts "id = " + id.to_s
      # Listen commands for command list 'id'
      listenUnityCommand(id, commands[id])
      puts "Listening (" + id.to_s + ")..."
      sleep 0.1
    end
  end
end

def doPlayerLoop(id,commands)
  puts "Player " + id.to_s
  # Set the live_loop name
  loopName = "playerLoop#{id.to_s}"
  # PLAYER LOOP
  live_loop loopName do
    puts "Playing (" + id.to_s + ")..."
    # Process command list number 'id'
    processCommands(id, commands[id])
    sleep 0.1
  end
end


'''
  ========================================================
    Variables and Initialization of the loop rack
  ========================================================
  '''
use_osc "localhost", 4560

# Number of loops listening to Unity commands
NUM_LOOPS = 4
# Command list
commands = []

# Command list initialization
for i in 0..(NUM_LOOPS - 1)
  puts i
  commands[i] = []
  doListenerLoop(i, commands) # Starts listening loop
  doPlayerLoop(i, commands)   # Starts playing loop
end


'''
  live_loop :listener1 do
    # Almacena en val lo que recibe por OSC desde Unity
    val = sync"/osc*/sonicpi/unity/trigger"
    puts "Received command " + val[0]
    # Lo añade al array de acciones
    actions.push(val)
    puts actions
  end

  live_loop :player1 do
    # Reproduce las acciones del array actions en orden
    actions.each do |action|
      puts action[0]
      case action[0]
      when "sleep"
        sleep 1
      when "play"
        play 89, amp: 0.1
      when "stop"
        stop
      else
        puts "ERROR"
      end
    end
    sleep 0.1
  end
  '''


