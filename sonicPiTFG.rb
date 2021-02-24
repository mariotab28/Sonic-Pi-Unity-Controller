'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo María Cidoncha Pérez and Mario Tabasco Vargas
Version 0.0
'''

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
  doListenerLoop(i, commands) # Starts listening loop
  doPlayerLoop(i, commands)   # Starts playing loop
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
  puts "hola " + id
  val = sync"/osc*/sonicpi/unity/trigger"
  puts "Received command " + val[0]
  # Add the message to commands
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
    puts "Processing: " + com[0]
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
      listenUnityCommand(0, commands)
      puts "Listening (" + id.to_s + ")..."
      sleep 0.1
    end
  end
end

def doPlayerLoop(id,commands)
  puts "Player " + id.to_s
  # PLAYER LOOP
  live_loop :playerLoop do
    processCommands(id, commands)
    puts "Playing (" + id.to_s + ")..."
    sleep 0.1
  end
  
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


